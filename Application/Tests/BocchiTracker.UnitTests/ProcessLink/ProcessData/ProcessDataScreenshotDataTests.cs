using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataScreenshotDataTests
    {
        [Fact]
        public async Task Test_Handle()
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

            var mediatorMock = new Mock<IMediator>();

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

            ModelEventBus.ReceiveScreenshotEvent? captured = null;
            mediatorMock
                 .Setup(x => x.Send(
                     It.IsAny<ModelEventBus.ReceiveScreenshotEvent>(),
                     It.IsAny<CancellationToken>()))
                 .Callback<ModelEventBus.ReceiveScreenshotEvent, CancellationToken>((data, token) =>
                 {
                     captured = data;
                 }
            );

            var process_data_screenshot = ProcessDataFactory.Create(Packet.GetRootAsPacket(buffer));
            Assert.NotNull(process_data_screenshot);
            await process_data_screenshot.Process(mediatorMock.Object, cClientID);

            Assert.NotNull(captured);
            Assert.NotNull(captured.ScreenshotData);

            Assert.Equal(cWidth, captured.ScreenshotData.Width);
            Assert.Equal(cHeight, captured.ScreenshotData.Height);
            Assert.Equal(cResource, captured.ScreenshotData.ImageData);
        }
    }
}
