using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Perfon.Core.Common;
using Perfon.Core.PerfCounters;
using Perfon.Core.PerfCounterStorages.LiteDbStorage;
using Perfon.Interfaces.Common;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// Driver for store/restore performance counter values in LiteDb.
    /// Fie names are one per date: perfCounters_yyyy-MM-dd.litedb
    /// </summary>
    public class PerfCounterLiteDbStorage : IPerfomanceCountersStorage
    {
        private string PathToDbFolder { get; set; }

        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<IPerfonErrorEventArgs> OnError;


        public PerfCounterLiteDbStorage(string pathToDbFolder)
        {
            PathToDbFolder = pathToDbFolder + "\\";
        }

        /// <summary>
        /// Awaitable.
        /// Stores perf counters into LiteDb file.
        /// One file per date
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        public Task StorePerfCounters(IEnumerable<IPerfCounterInputData> counters, DateTime? nowArg = null, string appId = null)
        {
            try
            {
                var now = DateTime.Now;
                if (nowArg.HasValue)
                {
                    now = nowArg.Value;
                }

                var dbName = GetDbWriteName(now);

                //var db = GetFromCacheOrCreate(dbName);
                using (var db = new LiteDatabase(dbName))
                {
                    var names = db.GetCollection("CounterNames");

                    using (var trans = db.BeginTrans())
                    {
                        // Get customer collection
                        foreach (var counter in counters)
                        {
                            try
                            {
                                var col = names.Find(Query.EQ("Name", counter.Name)).FirstOrDefault();
                                if (col == null)
                                {
                                    var doc = new BsonDocument();
                                    doc.Add("Name", counter.Name);
                                    names.Insert(doc);
                                }


                                int collId = counter.Name.GetHashCode();
                                int blockId = now.Hour * 12 + now.Minute / 5;
                                int docId = collId + blockId.GetHashCode();
                                var countersColl = db.GetCollection<PerfCountersDoc>(collId.ToString());
                                var docCounter = countersColl.Find(a => a._id == docId).FirstOrDefault();
                                if (docCounter == null)
                                {
                                    docCounter = new PerfCountersDoc();
                                    docCounter.CounterName = counter.Name;
                                    docCounter._id = docId;
                                    docCounter.blockId = (short)blockId;
                                }

                                docCounter.Values.Add(new PerfCounterLiteDbDataBlock(now, counter.Value));

                                countersColl.Upsert(docCounter);

                            }
                            catch (Exception exc)
                            {
                                if (OnError != null)
                                {
                                    OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                                }
                            }
                        }
                        trans.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                }
            }

            return Task.Delay(0);
        }

        public Task<IEnumerable<IPerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null, int skip = 0, string appId = null)
        {
            var list = new List<IPerfCounterValue>();

            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            date = date.Value.Date;

            try
            {
                //var db = GetFromCacheOrCreate(GetDbReadOnlyName(date.Value));
                using (var db = new LiteDatabase(GetDbReadOnlyName(date.Value)))
                {
                    try
                    {
                        int id = counterName.GetHashCode();
                        var countersColl = db.GetCollection<PerfCountersDoc>(id.ToString());
                        var res = countersColl.Find(a => a.CounterName == counterName).OrderBy(a => a.blockId);
                        if (res != null)
                        {
                            foreach (var doc in res)
                            {
                                list.AddRange(doc.Values.Select(a => a.GetValue(date.Value)).ToList());
                            }
                            list = list.Skip(skip).ToList();
                        }
                    }
                    catch (Exception exc)
                    {
                        if (OnError != null)
                        {
                            OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                }
            }

            return Task.FromResult(list as IEnumerable<IPerfCounterValue>);
        }

        public Task<IEnumerable<string>> GetCountersList()
        {
            var res = new List<string>();

            try
            {
                var now = DateTime.Now.Date;

                //var db = GetFromCacheOrCreate(GetDbReadOnlyName(now));
                using (var db = new LiteDatabase(GetDbReadOnlyName(now)))
                {
                    var names = db.GetCollection("CounterNames");
                    res = names.FindAll().Select(a => a["Name"].AsString).ToList();
                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new PerfonErrorEventArgs(exc.ToString()));
                }
            }

            return Task.FromResult(res as IEnumerable<string>);
        }





        private string GetDbName(DateTime date)
        {
            return PathToDbFolder + "perfCounters_" + date.ToString("yyyy-MM-dd") + ".litedb";
        }
        private string GetDbReadOnlyName(DateTime date)
        {
            //return  GetDbName(date);

            string journal = "";
            //if (date.Date != DateTime.Now.Date)
            {
                journal = ";Journal=false";
            }
            //;Mode=ReadOnly
            return "Filename=" + GetDbName(date) + journal + ";Mode=ReadOnly";//;Timeout=" + TimeSpan.FromSeconds(30);
        }
        private string GetDbWriteName(DateTime date)
        {
            return GetDbName(date);

            string journal = "";
            //if (date.Date != DateTime.Now.Date)
            {
                // journal = ";Journal=false";
            }
            return "Filename=" + GetDbName(date) + journal;
        }


        private LiteDatabase GetFromCacheOrCreate(string dbName)
        {
            LiteDatabase res = null;
            if (dbCache.TryGetValue(dbName, out res))
            {
                return res;
            }

            res = new LiteDatabase(dbName);

            //res = dbCache.GetOrAdd(dbName, res);

            return res;
        }

        private ConcurrentDictionary<string, LiteDatabase> dbCache = new ConcurrentDictionary<string, LiteDatabase>();

        public void Dispose()
        {
            foreach (var item in dbCache)
            {
                item.Value.Dispose();
            }
            dbCache.Clear();
        }

    }
}
