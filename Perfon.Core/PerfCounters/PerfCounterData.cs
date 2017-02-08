using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    public class PerfCounterData : IPerfCounterData
    {
        public PerfCounterData(string name, float value, string formattedValue) //, DateTime timeStamp)
        {
            Name = name;
            Value = value;
            FormattedValue = formattedValue;
            //TimeStamp = timeStamp;
        }

        public string Name { get; private set; }

        public string FormattedValue { get; private set; }

        public float Value { get; private set; }

        //public DateTime TimeStamp { get; private set; }
    }
}
