using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Perfon.Core.Notifications;
using Perfon.Interfaces.Notifications;
using Perfon.Interfaces.PerfCounters;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Base class for performance counters
    /// Processed to Per-Second-Value
    /// One could change perf counter value in a thread safe manner
    /// </summary>
    public abstract class PerformanceCounterBase : IPerformanceCounter
    {
        public PerformanceCounterBase(string name, float postProcessKoeff = 1, string formatString="n0")
        {
            Name = name;
            _value = 0;
            PostProcessMultiplyCoeff = postProcessKoeff;
            FormatString = formatString;

            Thresholds = new List<IThresholdNotification>();
        }

        /// <summary>
        /// Name of the perf counter
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Applied to return value
        /// </summary>
        public float PostProcessMultiplyCoeff { get; set; }

        /// <summary>
        /// Get Value as formatted string
        /// </summary>
        /// <returns></returns>
        public string GetFormattedValue()
        {
            return GetValue().ToString(FormatString);
        }

        /// <summary>
        /// Format for Valuse As Formatted String
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Increment perf counter value by 1
        /// Thread safe
        /// </summary>
        public virtual void Increment()
        {
            Interlocked.Increment(ref _value);
        }

        /// <summary>
        /// Used for calculation of value-per-sec
        /// </summary>
        public float ReversedPeriodValue { get; set; }

        /// <summary>
        /// Add a value to perf counter value
        /// Thread safe
        /// </summary>
        public virtual void Add(long addValue)
        {
            Interlocked.Add(ref _value, addValue);
        }

        /// <summary>
        /// Reset perf counter value to 0
        /// Thread safe
        /// </summary>
        public virtual void Reset(long? newPollingPeriod = null)
        {
            Interlocked.Exchange(ref _value, 0);

            if (newPollingPeriod.HasValue)
            {
                ReversedPeriodValue = 1.0f / newPollingPeriod.Value;
            }
        }

        /// <summary>
        /// Get current perf counter value
        /// </summary>
        /// <returns></returns>
        public virtual float GetValue()
        {
            if (ReversedPeriodValue != 0)
            {
                return PostProcessMultiplyCoeff * Value * ReversedPeriodValue;
            }
            else
            {
                return PostProcessMultiplyCoeff * Value;
            }
        }


        public IList<IThresholdNotification> Thresholds { get; private set; }


        public void AddThreshold(IThresholdNotification thr)
        {
            Thresholds.Add(thr);
        }

        /// <summary>
        /// Get current counter value, for pass it to Counter Storage
        /// </summary>
        /// <returns></returns>
        public IPerfCounterInputData GetPerfCounterData()
        {
            return new PerfCounterInputData(Name, GetValue(), GetFormattedValue());
        }

        /// <summary>
        /// Counter value, for internal usage
        /// </summary>
        protected long _value;

        protected long Value
        {
            get
            {
                return Interlocked.Read(ref _value);
            }
        }



    }
}
