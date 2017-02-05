using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Tracking max value
    /// Processed to Per-Second-Value
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public class PerformanceMaxCounter : PerformanceCounterBase
    {
        public PerformanceMaxCounter(string name, float postProcessKoeff = 1, string formatString = "n0")
            : base(name, postProcessKoeff, formatString)
        {
        }

        /// <summary>
        /// Replace current value if new vlaue is larger
        /// </summary>
        /// <param name="newValue"></param>
        public override void Add(long newValue)
        {
            if (newValue <= Value)
            {
                return;
            }
            var initialValue = Value;
            while (Interlocked.CompareExchange(ref _value, newValue, initialValue) != initialValue)
            {

            }
    
        }

        /// <summary>
        /// Add is the only way to change this caounter value
        /// Should it thorw exception?
        /// </summary>
        public override void Increment()
        {
            //base.Increment();
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public override double GetValue()
        {
            if (ReversedPeriodValue != 0)
            {
                return PostProcessMultiplyCoeff * Value;
            }
            else
            {
                return PostProcessMultiplyCoeff * Value;
            }
        }
        
    }
}
