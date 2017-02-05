using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfMonLib.Core
{
    /// <summary>
    /// User for reporting errors occured inside PerfMonLib to clients
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Description of the error
        /// </summary>
        public string Message { get; set; }

        public ErrorEventArgs(string message)
        {
            Message = message;
        }
    }
    
}
