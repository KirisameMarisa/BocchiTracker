using BocchiTracker.ModelEventBus;
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
            status["X"] = data.X.ToString();
            status["Y"] = data.Y.ToString();
            status["Z"] = data.Z.ToString();
            status["Stage"] = data.Stage;

            inEventAggregator
                .GetEvent<AppStatusQueryEvent>()
                .Publish(new AppStatusQueryEventParameter(new AppStatus
                {
                    QueryID = (int)QueryID.AppBasicInfo,
                    ClientID = inClientID,
                    Status = status
                }));
        }
    }
}
