using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfMonLib.Core.PerfCounters;

namespace PerfMonLib.Core.Notifications
{
    public class ThresholdMaxNotification : ThresholdBaseNotification
    {
        public ThresholdMaxNotification(double thresholdValue, string message = ""):base(thresholdValue, message)
        {
            if(string.IsNullOrEmpty(Message))
            {
                Message = "Theshould max value " + ThresholdValue+" ";
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
