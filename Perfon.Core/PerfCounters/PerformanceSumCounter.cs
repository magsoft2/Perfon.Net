using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Tracking growing up value
    /// Processed to Per-Second-Value
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public class PerformanceSumCounter : PerformanceCounterBase
    {
        public PerformanceSumCounter(string name, float postProcessKoeff = 1, string formatString = "n0")
            : base(name, postProcessKoeff, formatString)
        {
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public override float GetValue(bool resetAfterRead = false)
        {
            float res = PostProcessMultiplyCoeff * (float)Value * ReversedPeriodValue;
            if (ReversedPeriodValue == 0)
            {
                res = PostProcessMultiplyCoeff * (float)Value;
            }

            if (resetAfterRead)
            {
                Reset();
            }

            return res;
        }
        
    }
}
