using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Interfaces.PerfCounters;

namespace Perfon.Interfaces.Notifications
{
    /// <summary>
    /// Used for tracking threshold of perf counters.
    /// It raises notification about threshold violation
    /// </summary>
    public interface IThresholdNotification
    {
        float ThresholdValue { get; }

        bool IsThresholdViolated {get;}

        /// <summary>
        /// Message attached to every notification
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Returns true if perf counter violates the threshold
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        bool TestThresholdOk(IPerformanceCounter counter);

        /// <summary>
        /// Occurs when threshold goes from nonViolated to violated state
        /// </summary>
        event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolated;
        /// <summary>
        /// Occurs when threshold goes from Violated to non-violated state
        /// </summary>
        event EventHandler<IThreshouldNotificationEventArg> OnThresholdViolationRecovered;
    }
}
