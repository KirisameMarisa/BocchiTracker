using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataPlayerPosition : IProcessData
    {
        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsPlayerPosition();
            var status = new Dictionary<string, string>();
            status["PlayerPosition.x"] = data.X.ToString();
            status["PlayerPosition.y"] = data.Y.ToString();
            status["PlayerPosition.z"] = data.Z.ToString();
            status["PlayerPosition.stage"] = data.Stage;

            inEventAggregator
                .GetEvent<AppStatusQueryEvent>()
                .Publish(new AppStatusQueryEventParameter(new AppStatus
                {
                    QueryID = (int)QueryID.PlayerPosition,
                    ClientID = inClientID,
                    Status = status
                }));
        }
    }
}
