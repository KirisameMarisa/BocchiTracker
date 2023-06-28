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
            var screenshot_data = new ModelEventBus.ScreenshotData { Width = _data.Width, Height = _data.Height };
            screenshot_data.ImageData = new byte[_data.Width * _data.Height * 4];
            Array.Copy(_data.GetDataArray(), screenshot_data.ImageData, _data.DataLength);
            
            await inMediator.Send(new ModelEventBus.ReceiveScreenshotEvent(screenshot_data));
        }
    }
}
