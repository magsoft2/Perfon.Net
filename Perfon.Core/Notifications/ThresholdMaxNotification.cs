using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Core.PerfCounters;
using Perfon.Interfaces.PerfCounters;

namespace Perfon.Core.Notifications
{
    /// <summary>
    /// Check threshold max value and raise notification about violation
    /// </summary>
    public class ThresholdMaxNotification : ThresholdBaseNotification
    {
        public ThresholdMaxNotification(float thresholdValue, string message = "")
            : base(thresholdValue, message)
        {
            if(string.IsNullOrEmpty(Message))
            {
                Message = "Theshold max value " + ThresholdValue+" ";
            }
        }

        public override bool TestThresholdOk(IPerformanceCounter counter)
        {
            bool res = true;

            var val = counter.GetValue();

            res = val >= ThresholdValue;

            if (res && !IsThresholdViolated)
            {
                IsThresholdViolated = true;
                RaiseThresholdViolated(new ThreshouldNotificationEventArg(counter.Name+ ", "+ Message+" violated, val="+val));
            }

            if (!res && IsThresholdViolated)
            {
                IsThresholdViolated = false;
                RaiseThresholdViolated(new ThreshouldNotificationEventArg(counter.Name + ", " + Message + " recovered, val=" + val));
            }

            return res;
        }

        

    }
}
