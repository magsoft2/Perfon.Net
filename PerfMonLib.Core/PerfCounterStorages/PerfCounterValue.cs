using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfMonLib.Core.PerfCounterStorages
{
    /// <summary>
    /// Used for obtaining perf counter time series
    /// 
    /// </summary>
    public struct PerfCounterValue
    {
        //public PerfCounterValue()
        //{

        //}

        public PerfCounterValue(DateTime timestamp, double value):this()
        {
            Timestamp = timestamp;
            Value = value;
        }

        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}
