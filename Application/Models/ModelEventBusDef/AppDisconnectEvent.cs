using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class AppDisconnectEventParameter
    {
        public int ClientID { get; set; }

        public AppDisconnectEventParameter(int inClientID)
        {
            ClientID = inClientID;
        }
    }

    public class AppDisconnectEvent : PubSubEvent<AppDisconnectEventParameter> {}
}
