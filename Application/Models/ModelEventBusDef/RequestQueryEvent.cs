using MediatR;
using BocchiTracker.ProcessLinkQuery.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class RequestQueryEvent : IRequest
    {
        public int ClientID { get; set; }

        public QueryID QueryID { get; set; }

        public RequestQueryEvent(int inClientID, QueryID inQueryID)
        {
            ClientID = inClientID;
            QueryID = inQueryID;
        }
    }
}
