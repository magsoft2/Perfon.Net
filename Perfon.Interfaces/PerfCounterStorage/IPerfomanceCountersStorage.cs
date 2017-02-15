using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Interfaces.Common;

namespace Perfon.Interfaces.PerfCounterStorage
{
    /// <summary>
    /// Interface should be implemented by storage drivers.
    /// </summary>
    public interface IPerfomanceCountersStorage
    {
        /// <summary>
        /// Store a list of perf counter values,
        /// with optional TimeStamp and AppId
        /// Driver should try ro use bulk insert
        /// Awaitable
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        Task StorePerfCounters(IEnumerable<IPerfCounterInputData> counters, DateTime? timeStamp = null, string appId = null);

        /// <summary>
        /// Get counter history track for specified date
        /// Skip is used for periodic polling, allowing to get only recent values not recieved yet
        /// Awaitable
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IEnumerable<IPerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null, int skip = 0, string appId = null);

        /// <summary>
        /// Get list of names of all perf counters in the storage
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCountersList();

        /// <summary>
        /// Reports about errors and exceptions occured in the storage driver
        /// </summary>
        event EventHandler<IPerfonErrorEventArgs> OnError;
    }

}
