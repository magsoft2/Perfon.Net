using System;

namespace Perfon.Interfaces.Common
{
    /// <summary>
    /// User for reporting errors occured inside PerfMonLib to clients
    /// </summary>
    public interface IPerfonErrorEventArgs
    {
        string Message { get; set; }
    }
}
