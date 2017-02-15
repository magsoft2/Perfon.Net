using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;
using Perfon.Interfaces.Notifications;
using Perfon.Interfaces.PerfCounters;

namespace Perfon.Core.Notifications
{
    /// <summary>
    /// Abstract class implementing base functional needed by derivers
    /// </summary>
    public abstract class ThresholdBaseNotification : IThresholdNotification
    {
        public ThresholdBaseNotification(float thresholdValue, string message = "")
        {
            ThresholdValue = thresholdValue;
            Message = message;

            if(string.IsNullOrEmpty(Message))
            {
                Message = "Violated theshold " + ThresholdValue+" :";
            }
        }

        public float ThresholdValue { get; private set; }

        public bool IsThresholdViolated { get; protected set; }

        /// <summary>
        /// Function called by perfofmance counters to check if threshold is ok
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        public abstract bool TestThresholdOk(IPerformanceCounter counter);

        /// <summary>
        /// Event raised after threshold  violation
        /// </summary>
        public event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolated;

        /// <summary>
        /// Event raised after threshold violation is gone
        /// </summary>
        public event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolationRecovered;

        /// <summary>
        /// Fixed part of information passed to subscribers
        /// </summary>
        public string Message { get; protected set; }


        protected void RaiseThresholdViolated(ThreshouldNotificationEventArg arg)
        {
            if (OnThresholdViolated != null)
            {
                OnThresholdViolated(new object(), arg);
            }
        }
        protected void RaiseThresholdViolationRecovered(ThreshouldNotificationEventArg arg)
        {
            if (OnThresholdViolationRecovered != null)
            {
                OnThresholdViolationRecovered(new object(), arg);
            }
        }

        
    }
}
