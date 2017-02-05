using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;

namespace Perfon.Core.PerfCounterStorages
{
    /// <summary>
    /// Interface should be implemented by storage drivers.
    /// </summary>
    public interface IPerfomanceCountersStorage
    {
        /// <summary>
        /// Store a list of perf counter values
        /// </summary>
        /// <param name="counters"></param>
        /// <returns></returns>
        Task StorePerfCounters(IEnumerable<IPerfCounterData> counters);

        /// <summary>
        /// Get counter history track for specified date
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<IEnumerable<PerfCounterValue>> QueryCounterValues(string counterName, DateTime? date = null);

        /// <summary>
        /// Get list of names of all perf counters in the storage
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCountersList();
    }

}
