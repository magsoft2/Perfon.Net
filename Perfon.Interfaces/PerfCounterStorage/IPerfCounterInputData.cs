using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfon.Interfaces.PerfCounterStorage
{
    /// <summary>
    /// Performance Counter Data for passing to storage.
    /// Immutable.
    /// </summary>
    public interface IPerfCounterInputData
    {
        /// <summary>
        /// Name of Perf. Counter which generate this data block
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Value of Perf. Counter
        /// </summary>
        float Value { get; }

        /// <summary>
        /// Optional.
        /// String Formatted Value of perf. Counter Data
        /// Used for CVS Storage for example
        /// </summary>
        string FormattedValue { get; }
        //DateTime TimeStamp { get; }
    }
}
