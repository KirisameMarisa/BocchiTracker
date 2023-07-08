using BocchiTracker.ProcessLinkQuery.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    internal class ProcessDataScreenshotData : IProcessData
    {
        private readonly ScreenshotData _data;

        public ProcessDataScreenshotData(ScreenshotData inData)
        {
            _data = inData;
        }

        public async Task Process(IMediator inMediator, int inClientID)
        {
            var screenshotData = new ModelEventBus.ScreenshotData { Width = _data.Width, Height = _data.Height };
            screenshotData.ImageData = new byte[_data.Width * _data.Height * 4];
            Array.Copy(_data.GetDataArray(), screenshotData.ImageData, _data.DataLength);
            
            await inMediator.Send(new ModelEventBus.ReceiveScreenshotEvent(screenshotData));
        }
    }
}
