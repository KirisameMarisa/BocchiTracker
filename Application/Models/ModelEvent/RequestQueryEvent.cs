using BocchiTracker.ProcessLinkQuery.Queries;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class RequestEventParameterBase
    {
        public int ClientID { get; set; }

        public QueryID QueryID { get; set; }

        public RequestEventParameterBase(int clientID, QueryID queryID)
        {
            ClientID = clientID;
            QueryID = queryID;
        }
    }

    public class ScreenshotRequestEventParameter : RequestEventParameterBase
    {
        public ScreenshotRequestEventParameter(int inClientID) : base(inClientID, QueryID.ScreenshotRequest) { }
    }

    public class JumpRequestEventParameter : RequestEventParameterBase 
    {
        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public string Stage { get; set; }

        public JumpRequestEventParameter(int inClientID, float inX, float inY, float inZ, string inStage) : base(inClientID, QueryID.JumpRequest) 
        {
            PosX = inX;
            PosY = inY;
            PosZ = inZ;
            Stage = inStage;
        }
    }

    public class IssuesRequestEventParameter : RequestEventParameterBase
    {
        public List<TicketData> TicketData { get; set; } = new List<TicketData>();

        public IssuesRequestEventParameter(int inClientID) : base(inClientID, QueryID.IssueesRequest) {}
    }

    public class RequestQueryEvent : PubSubEvent<RequestEventParameterBase> {}
}
