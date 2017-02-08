using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// Driver for store/restore performance counter values in memory
    /// </summary>
    public class PerfCounterInMemoryCacheStorage:IPerfomanceCountersStorage
    {
        private long ExpirationInSeconds { get; set; }

        private MemoryCache MemoryDb { get; set; }

        private StringCollection counterNames { get; set; }

        public PerfCounterInMemoryCacheStorage(long expirationInSeconds=60*60)
        {
            ExpirationInSeconds = expirationInSeconds;
            MemoryDb = new MemoryCache("PerfCounterInMemoryCacheStorage");
            counterNames = new StringCollection();
        }

        class DataRowPerTimeStamp
        {
            public DataRowPerTimeStamp(DateTime timeStamp)
            {
                TimeStamp = timeStamp;
                CountersValue = new Dictionary<string, float>();
            }

            public DateTime TimeStamp { get; private set; }

            private Dictionary<string, float> CountersValue { get; set; }

            /// <summary>
            /// Returns NaN if perf counter name is not found
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            internal float TryGetCounterValue(string name)
            {
                float res = float.NaN;
                CountersValue.TryGetValue(name, out res);
                return res;
            }

            internal void AddCounterValue(string key, float value)
            {
                CountersValue[key] = value;
            }
        }

        /// <summary>
        /// Awaitable.
        /// Stores perf counters into memory
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        public async Task StorePerfCounters(IEnumerable<IPerfCounterData> counters)
        {
            try
            {
                var sb = new StringBuilder();

                DateTime timeStamp = DateTime.Now;

                DateTimeOffset dateExpiration = timeStamp.AddSeconds(ExpirationInSeconds);

                var timeStampStr = timeStamp.ToString("hh:mm:ss.fff");

                foreach (var item in counters)
                {
                    var row = MemoryDb.Get(timeStampStr) as DataRowPerTimeStamp;
                    if(row == null)
                    {
                        row = new DataRowPerTimeStamp(timeStamp);
                        MemoryDb.Add(timeStampStr, row, dateExpiration);
                    }

                    row.AddCounterValue(item.Name, item.Value);

                    counterNames.Add(item.Name);
                }

            }
            catch (Exception exc)
            {
                if (OnError != null)
                {
                    OnError(new object(), new ErrorEventArgs(exc.ToString()));
                }
            }
        }

        public Task<IEnumerable<PerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            var list = new List<PerfCounterValue>();

            var all = MemoryDb.ToList();

            foreach (var item in all)
            {   
                var row = item.Value as DataRowPerTimeStamp;
                var res = row.TryGetCounterValue(counterName);
                if (!double.IsNaN(res) && row.TimeStamp.Date == date.Value.Date)
                {
                    list.Add(new PerfCounterValue(row.TimeStamp, res));
                }
            }

            return Task.FromResult(list as IEnumerable<PerfCounterValue>);
        }

        public Task<IEnumerable<string>> GetCountersList()
        {
            return Task.FromResult(counterNames.OfType<string>());
        }


        /// <summary>
        /// Reports about errors and exceptions occured.
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;


    }
}
