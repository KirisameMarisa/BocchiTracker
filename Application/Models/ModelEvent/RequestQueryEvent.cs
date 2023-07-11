using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class RequestQueryEventParameter
    {
        public int ClientID { get; set; }

        public QueryID QueryID { get; set; }

        public RequestQueryEventParameter(int inClientID, QueryID inQueryID)
        {
            ClientID = inClientID;
            QueryID = inQueryID;
        }
    }

    public class RequestQueryEvent : PubSubEvent<RequestQueryEventParameter> {}
}
