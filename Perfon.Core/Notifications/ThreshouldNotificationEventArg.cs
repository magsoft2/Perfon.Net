using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perfon.Core.Notifications
{
    public class ThreshouldNotificationEventArg:EventArgs
    {
        public string Message { get; private set; }

        public ThreshouldNotificationEventArg(string message)
        {
            Message = message;
        }
    }
}
