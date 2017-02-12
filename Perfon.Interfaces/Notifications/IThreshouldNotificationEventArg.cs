using System;

namespace Perfon.Interfaces.Notifications
{
    /// <summary>
    /// Agrument of Perf. Counter Threshold violation Notification
    /// </summary>
    public interface IThreshouldNotificationEventArg
    {
        string Message { get; }
    }
}
