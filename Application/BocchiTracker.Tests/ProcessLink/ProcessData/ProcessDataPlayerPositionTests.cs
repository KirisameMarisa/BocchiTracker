using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
using Moq;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataPlayerPositionTests
    {
        [Fact]
        public async Task Test_Handle()
        {
            const int cClientID = 9999;

            var mediatorMock = new Mock<IMediator>();

            var fbb = new FlatBufferBuilder(1024);
            const string cStage = "xUnitTest Stage";
            var stage = fbb.CreateString(cStage);
            var PosX = 100;
            var PosY = 200;
            var PosZ = 300;

            PlayerPosition.StartPlayerPosition(fbb);
            PlayerPosition.AddStage(fbb, stage);
            PlayerPosition.AddX(fbb, PosX);
            PlayerPosition.AddY(fbb, PosY);
            PlayerPosition.AddZ(fbb, PosZ);
            var table = PlayerPosition.EndPlayerPosition(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, QueryID.PlayerPosition);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);

            var buffer = fbb.DataBuffer;

            ModelEventBus.AppStatus? captured = null;
            mediatorMock
                 .Setup(x => x.Send(
                     It.IsAny<ModelEventBus.AppStatusQueryEvent>(),
                     It.IsAny<CancellationToken>()))
                 .Callback<ModelEventBus.AppStatusQueryEvent, CancellationToken>((appStatus, token) =>
                 {
                     captured = appStatus.AppStatus;
                 }
            );

            var appStatusQuery = ProcessDataFactory.Create(Packet.GetRootAsPacket(buffer));
            Assert.NotNull(appStatusQuery);
            await appStatusQuery.Process(mediatorMock.Object, cClientID);

            Assert.NotNull(captured);
            Assert.Equal((byte)QueryID.PlayerPosition, captured?.QueryID);
            Assert.Equal(cClientID, captured?.ClientID);

            var status = captured?.Status as Dictionary<string, dynamic>;
            Assert.NotNull(status);

            Assert.Equal(cStage, (string)status["Stage"]);
            Assert.Equal(PosX, (float)status["X"]);
            Assert.Equal(PosY, (float)status["Y"]);
            Assert.Equal(PosZ, (float)status["Z"]);
        }
    }
}
