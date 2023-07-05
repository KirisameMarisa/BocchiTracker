using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class AppStatus
    {
        public byte QueryID { get; set; }

        public int ClientID { get; set; }

        public Dictionary<string, string> Status { get; set; } = new Dictionary<string, string>();
    }

    public class AppStatusQueryEvent : IRequest 
    {
        public AppStatus AppStatus { get; private set; }

        public AppStatusQueryEvent(AppStatus inAppStatus)
        {
            AppStatus = inAppStatus;
        }
    }
}
