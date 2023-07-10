using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataAppBasicInfo : IProcessData
    {
        public async Task Process(IMediator inMediator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsAppBasicInfo();
            var status = new Dictionary<string, string>();
            status["Pid"] = data.Pid.ToString();
            status["AppName"] = data.AppName;
            status["Args"] = data.Args;
            status["Platform"] = data.Platform;

            await inMediator.Send(new ModelEventBus.AppStatusQueryEvent(new ModelEventBus.AppStatus 
            {
                QueryID = (int)QueryID.AppBasicInfo,
                ClientID = inClientID,
                Status = status
            }));
        }
    }
}
