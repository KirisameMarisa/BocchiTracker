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

        public Dictionary<string, dynamic> Status { get; set; } = new Dictionary<string, dynamic>();
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
