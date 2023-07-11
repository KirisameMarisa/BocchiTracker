using BocchiTracker.ModelEventBus;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataScreenshotData : IProcessData
    {
        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsScreenshotData();
            var imageData = new byte[data.Width * data.Height * 4];
            Array.Copy(data.GetDataArray(), imageData, data.DataLength);
            
            inEventAggregator
                .GetEvent<ReceiveScreenshotEvent>()
                .Publish(new ReceiveScreenshotEventParameter(data.Width, data.Height, imageData));
        }
    }
}
