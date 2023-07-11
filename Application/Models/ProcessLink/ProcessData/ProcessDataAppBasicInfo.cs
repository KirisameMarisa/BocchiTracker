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
    public class ProcessDataAppBasicInfo : IProcessData
    {
        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsAppBasicInfo();
            var status = new Dictionary<string, string>();
            status["Pid"] = data.Pid.ToString();
            status["AppName"] = data.AppName;
            status["Args"] = data.Args;
            status["Platform"] = data.Platform;

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
