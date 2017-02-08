using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Perfon.Core.PerfCounters;

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
        public event EventHandler<ErrorEventArgs> OnError;


        public PerfCounterLiteDbStorage(string pathToDbFolder)
        {
            PathToDbFolder = pathToDbFolder+"\\";
        }

        /// <summary>
        /// Awaitable.
        /// Stores perf counters into file as a line
        /// Add headers if file doesn't exists
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        public Task StorePerfCounters(IEnumerable<IPerfCounterData> counters)
        {
            try
            {
                var now = DateTime.Now;

                var dbName = GetDbName(now);

                using (var db = new LiteDatabase(PathToDbFolder + dbName))
                {
                    // Get customer collection
                    foreach (var counter in counters)
                    {
                        try
                        {
                            var countersColl = db.GetCollection<PerfCounterValue>(counter.Name.GetHashCode().ToString());
                            var names = db.GetCollection("CounterNames");

                            // Index document using a document property
                            //countersColl.EnsureIndex("Timestamp", true);

                            var id = names.Find(Query.EQ("Name", counter.Name)).FirstOrDefault();
                            if (id == null)
                            {
                                var doc = new BsonDocument();
                                doc.Add("Name", counter.Name);
                                names.Insert(doc);
                            }

                            var item = new PerfCounterValue(now, counter.Value);

                            countersColl.Insert(item);
                        }
                        catch (Exception exc)
                        {
                            if (OnError != null)
                            {
                                OnError(new object(), new ErrorEventArgs(exc.ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new ErrorEventArgs(exc.ToString()));
                }
            }

            return Task.Delay(0);
        }

        public Task<IEnumerable<PerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null)
        {
            var list = new List<PerfCounterValue>();

            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            date = date.Value.Date;

            try
            {
                var dbName = GetDbName(date.Value);

                using (var db = new LiteDatabase(PathToDbFolder+dbName))
                {
                    var countersColl = db.GetCollection<PerfCounterValue>(counterName.GetHashCode().ToString());

                    list = countersColl.FindAll().Where(a => a.Timestamp.Date == date).ToList();

                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new ErrorEventArgs(exc.ToString()));
                }
            }

            return Task.FromResult(list as IEnumerable<PerfCounterValue>);
        }

        public Task<IEnumerable<string>> GetCountersList()
        {
            var res = new List<string>();

            try
            {
                var now = DateTime.Now;
                
                var dbName = GetDbName(now);

                using (var db = new LiteDatabase(PathToDbFolder + dbName))
                {
                    // Get customer collection
                    var names = db.GetCollection("CounterNames");

                    // Index document using a document property
                    //countersColl.EnsureIndex("Timestamp", true);

                    res = names.FindAll().Select(a => a["Name"].AsString).ToList();

                }
            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new ErrorEventArgs(exc.ToString()));
                }
            }

            return Task.FromResult(res as IEnumerable<string>);
        }





        private static string GetDbName(DateTime now)
        {
            return "perfCounters_" + now.ToString("yyyy-MM-dd") + ".litedb";
        }

        
    }
}
