using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Return the difference between two measures
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public class PerformanceDifferenceCounter : PerformanceCounterBase
    {
        public PerformanceDifferenceCounter(string name, float postProcessKoeff = 1, string formatString = "n0")
            : base(name, postProcessKoeff, formatString)
        {
        }


        /// <summary>
        /// </summary>
        public override void Increment()
        {
            
        }

        /// <summary>
        /// Add new value to perf counter value. It replaces prev value
        /// Thread safe
        /// </summary>
        public override void Add(long addValue)
        {
            ///Not very good, better to do - lock the whole function body
            Interlocked.Exchange(ref prevValue, Value);
            Interlocked.Exchange(ref _value, addValue);
        }

        /// <summary>
        /// No reset for this counter. Use Add instead (As SetNewValue)
        /// Thread safe
        /// </summary>
        public override void Reset(long? newPollingPeriod = null)
        {
            //base.Reset(newReversedPollingPeriod);

            //Interlocked.Exchange(ref prevValue, 0);
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public override float GetValue()
        {
             return PostProcessMultiplyCoeff * (Value - Interlocked.Read(ref prevValue));
        }

        private long prevValue = 0;

    }
}
