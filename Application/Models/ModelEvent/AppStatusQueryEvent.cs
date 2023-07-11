using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class AppStatus
    {
        public byte QueryID { get; set; }

        public int ClientID { get; set; }

        public Dictionary<string, string> Status { get; set; } = new Dictionary<string, string>();
    }

    public class AppStatusQueryEventParameter
    {
        public AppStatus AppStatus { get; private set; }

        public AppStatusQueryEventParameter(AppStatus inAppStatus) 
        {
            AppStatus = inAppStatus;
        }
    }

    public class AppStatusQueryEvent : PubSubEvent<AppStatusQueryEventParameter> { } 
}
