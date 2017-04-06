﻿using System;
using System.Collections.Generic;
using Perfon.Interfaces;
using Perfon.Interfaces.Notifications;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Interfaces.PerfCounters
{
    /// <summary>
    /// Common perf counter interface
    /// </summary>
    public interface IPerformanceCounter
    {
        /// <summary>
        /// Name of the counter
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Used for calculating of value-per-sec
        /// could be changed in resetfor the next polling cycle
        /// </summary>
        float ReversedPeriodValue { get; set; }

        /// <summary>
        /// Applied to returned value
        /// </summary>
        float PostProcessMultiplyCoeff { get; set; }

        /// <summary>
        /// Add a value
        /// Thread safe
        /// </summary>
        /// <param name="addValue"></param>
        void Add(long addValue);

        /// <summary>
        /// Get current counter value
        /// Could be processed in the overiding classes
        /// Thread safe
        /// </summary>
        /// <returns></returns>
        float GetValue(bool resetAfterRead = false);

        /// <summary>
        /// Return value as formatted string, using FormatString
        /// </summary>
        /// <returns></returns>
        string GetFormattedValue();

        /// <summary>
        /// Used for generate formatted string
        /// </summary>
        string FormatString { get; set; }

        /// <summary>
        /// Registered thresholds
        /// </summary>
        IList<IThresholdNotification> Thresholds { get; }

        /// <summary>
        /// Add threshold
        /// </summary>
        /// <param name="thr"></param>
        void AddThreshold(IThresholdNotification thr);


        /// <summary>
        /// Increment counter value by 1
        /// Thread safe
        /// </summary>
        void Increment();

        /// <summary>
        /// Reset counter value to 0.
        /// TODO: pass new poll period for adaptive polling.
        /// Poll period is fixed between polls.
        /// Thread safe
        /// </summary>
        void Reset(long? newPollingPeriod = null);

        /// <summary>
        /// Get current counter value, for pass it to Counter Storage
        /// </summary>
        /// <returns></returns>
        IPerfCounterInputData GetPerfCounterData(bool resetAfterRead = false);
    }
}
