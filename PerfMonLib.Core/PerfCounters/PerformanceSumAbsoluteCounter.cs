using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerfMonLib.Core.PerfCounters
{
    /// <summary>
    /// Tracking growing up value
    /// NOT Processed to Per-Second-Value
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public class PerformanceSumAbsoluteCounter : PerformanceCounterBase
    {
        public PerformanceSumAbsoluteCounter(string name, float postProcessKoeff = 1, string formatString = "n0")
            : base(name, postProcessKoeff, formatString)
        {
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            return PostProcessMultiplyCoeff * Value;
        }
        
    }
}
