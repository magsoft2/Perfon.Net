using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;

namespace Perfon.Core.Notifications
{
    public abstract class ThresholdBaseNotification : IThresholdNotification
    {
        public ThresholdBaseNotification(double thresholdValue, string message = "")
        {
            ThresholdValue = thresholdValue;
            Message = message;

            if(string.IsNullOrEmpty(Message))
            {
                Message = "Violated theshould " + ThresholdValue+" :";
            }
        }

        public double ThresholdValue { get; private set; }

        public bool IsThresholdViolated { get; protected set; }

        public abstract bool TestThresholdOk(IPerformanceCounter counter);

        public event EventHandler<ThreshouldNotificationEventArg> OnThresholdViolated;

        public event EventHandler<ThreshouldNotificationEventArg> OnThresholdViolationRecovered;

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

        public string Message { get; protected set; }
    }
}
