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
    public class ProcessDataPlayerPosition : IProcessData
    {
        public async Task Process(IMediator inMediator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsPlayerPosition();
            var status = new Dictionary<string, string>();
            status["X"] = data.X.ToString();
            status["Y"] = data.Y.ToString();
            status["Z"] = data.Z.ToString();
            status["Stage"] = data.Stage;

            await inMediator.Send(new ModelEventBus.AppStatusQueryEvent(new ModelEventBus.AppStatus
            {
                QueryID = (int)QueryID.PlayerPosition,
                ClientID = inClientID,
                Status = status
            }));
        }
    }
}
