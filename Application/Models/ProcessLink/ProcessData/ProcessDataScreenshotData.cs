using BocchiTracker.ProcessLinkQuery.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataScreenshotData : IProcessData
    {
        public async Task Process(IMediator inMediator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsScreenshotData();
            var screenshotData = new ModelEventBus.ScreenshotData { Width = data.Width, Height = data.Height };
            screenshotData.ImageData = new byte[data.Width * data.Height * 4];
            Array.Copy(data.GetDataArray(), screenshotData.ImageData, data.DataLength);
            
            await inMediator.Send(new ModelEventBus.ReceiveScreenshotEvent(screenshotData));
        }
    }
}
