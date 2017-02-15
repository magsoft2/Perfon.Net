using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Interfaces;
using Perfon.Interfaces.PerfCounterStorage;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Data for passing to Storages
    /// </summary>
    public class PerfCounterInputData : IPerfCounterInputData
    {
        public PerfCounterInputData(string name, float value, string formattedValue = "")
        {
            Name = name;
            Value = value;
            FormattedValue = formattedValue;
        }

        /// <summary>
        /// Counter name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string FormattedValue { get; private set; }

        /// <summary>
        /// Counter value
        /// </summary>
        public float Value { get; set; }

    }
}
