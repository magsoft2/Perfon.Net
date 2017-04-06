using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Tracking growing up value, returns an averaged per poll period value
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public class PerformanceAverageCounter : PerformanceCounterBase
    {
        public PerformanceAverageCounter(string name, float postProcessKoeff = 1, string formatString = "n0")
            : base(name, postProcessKoeff, formatString)
        {
        }


        /// <summary>
        /// Increment perf counter value by 1
        /// Thread safe
        /// </summary>
        public override void Increment()
        {
            base.Increment();

            Interlocked.Increment(ref counter);
        }

        /// <summary>
        /// Add a value to perf counter value
        /// Thread safe
        /// </summary>
        public override void Add(long addValue)
        {
            base.Add(addValue);
            Interlocked.Increment(ref counter);
        }

        /// <summary>
        /// Reset perf counter value to 0
        /// Thread safe
        /// </summary>
        public override void Reset(long? newPollingPeriod = null)
        {
            base.Reset(newPollingPeriod);

            Interlocked.Exchange(ref counter, 0);
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public override float GetValue(bool resetAfterRead = false)
        {
            float res = 0;

            if (counter > 0)
            {
                res = PostProcessMultiplyCoeff * Value / counter;
            }
            else
            {
                res = PostProcessMultiplyCoeff * Value;
            }

            if (resetAfterRead)
            {
                Reset();
            }

            return res;
        }

        private long counter = 0;

    }
}
