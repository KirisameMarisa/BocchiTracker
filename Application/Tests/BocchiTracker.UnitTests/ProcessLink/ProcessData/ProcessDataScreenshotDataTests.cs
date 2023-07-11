using BocchiTracker.ModelEventBus;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using Moq;
using Prism.Events;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataScreenshotDataTests
    {
        [Fact]
        public void Test_Handle()
        {
            const int cClientID = 9999;
            const int cWidth = 4;
            const int cHeight = 3;
            byte[] cResource =
            {
                  0,   0,   0,   0,   
                  0,   0,   0,   0, 
                255, 255, 255, 255,  
                255, 255,   0, 255,   
                  0,   0,   0,   0, 
                255,   0, 255,   0, 
                255, 255,   0, 255,
                  0, 255,   0, 255, 
                255,   0, 255,   0, 
                255,   0,   0,   0,    
                  0, 255,   0, 255, 
                255, 255, 255,  255,
            };

            var mockedEvent = new Mock<ReceiveScreenshotEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock
                .Setup(x => x.GetEvent<ReceiveScreenshotEvent>())
                .Returns(mockedEvent.Object);

            var serviceProcessData = new ServiceProcessData();
            serviceProcessData.Register(QueryID.ScreenshotData, new ProcessDataScreenshotData());

            var fbb = new FlatBufferBuilder(1);
            var offset_image_data = ScreenshotData.CreateDataVector(fbb, cResource);

            ScreenshotData.StartScreenshotData(fbb);
            ScreenshotData.AddHeight(fbb, cHeight);
            ScreenshotData.AddWidth(fbb, cWidth);
            ScreenshotData.AddData(fbb, offset_image_data);
            var table = ScreenshotData.EndScreenshotData(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, QueryID.ScreenshotData);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);
            
            var buffer = fbb.DataBuffer;
            serviceProcessData.Process(eventAggregatorMock.Object, cClientID, Packet.GetRootAsPacket(buffer));

            // Assert
            mockedEvent.Verify(x => x.Publish(It.Is<ReceiveScreenshotEventParameter>(
                   m => m.Width == cWidth
                && m.Height == cHeight
                && m.ImageData.SequenceEqual(cResource)
            )));
        }
    }
}
