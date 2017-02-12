using System;

namespace Perfon.Interfaces.PerfCounterStorage
{
    /// <summary>
    /// Performance Counter Data return by Storages
    /// Storage returns a collection of such data items for perf. counter track record.
    /// 
    /// </summary>
    public interface IPerfCounterValue
    {
        DateTime Timestamp { get; }

        float Value { get; }
    }
}
