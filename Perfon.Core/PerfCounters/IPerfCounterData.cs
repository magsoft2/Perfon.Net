using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfon.Core.PerfCounters
{
    /// <summary>
    /// Interface for pass immutable perf counter data to storage, etc
    /// </summary>
    public interface IPerfCounterData
    {
        string Name { get; }
        double Value { get; }

        string FormattedValue { get; }
        //DateTime TimeStamp { get; }
    }
}
