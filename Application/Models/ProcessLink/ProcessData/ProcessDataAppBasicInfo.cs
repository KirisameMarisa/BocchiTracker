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
        private readonly AppBasicInfo _data;

        public ProcessDataAppBasicInfo(AppBasicInfo inData)
        {
            _data = inData;
        }

        public async Task Process(IMediator inMediator, int inClientID)
        {
            var status = new Dictionary<string, string>();
            status["Pid"] = _data.Pid.ToString();
            status["AppName"] = _data.AppName;
            status["Args"] = _data.Args;
            status["Platform"] = _data.Platform;

            await inMediator.Send(new ModelEventBus.AppStatusQueryEvent(new ModelEventBus.AppStatus 
            {
                QueryID = (int)QueryID.AppBasicInfo,
                ClientID = inClientID,
                Status = status
            }));
        }
    }
}
