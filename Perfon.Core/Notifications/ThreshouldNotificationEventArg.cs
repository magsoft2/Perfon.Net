using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perfon.Interfaces.Notifications;

namespace Perfon.Core.Notifications
{
    /// <summary>
    /// Event arg for Notification events
    /// </summary>
    public class ThreshouldNotificationEventArg:EventArgs, IThreshouldNotificationEventArg
    {
        public string Message { get; private set; }

        public ThreshouldNotificationEventArg(string message)
        {
            Message = message;
        }
    }
}
