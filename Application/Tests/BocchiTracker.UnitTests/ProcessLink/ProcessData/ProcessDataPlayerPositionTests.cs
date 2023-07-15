using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using Moq;
using Prism.Events;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataPlayerPositionTests
    {
        [Fact]
        public void Test_Handle()
        {
            const int cClientID = 9999;

            var mockedEvent = new Mock<AppStatusQueryEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock
                .Setup(x => x.GetEvent<AppStatusQueryEvent>())
                .Returns(mockedEvent.Object);

            var serviceProcessData = new ServiceProcessData();
            serviceProcessData.Register(QueryID.PlayerPosition, new ProcessDataPlayerPosition());

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
            serviceProcessData.Process(eventAggregatorMock.Object, cClientID, Packet.GetRootAsPacket(buffer));

            // Assert
            mockedEvent.Verify(x => x.Publish(It.Is<AppStatusQueryEventParameter>(
                   m => m.AppStatus.ClientID == cClientID
                && m.AppStatus.QueryID == (byte)QueryID.PlayerPosition
                && m.AppStatus.Status["X"] == PosX.ToString()
                && m.AppStatus.Status["Y"] == PosY.ToString()
                && m.AppStatus.Status["Z"] == PosZ.ToString()
                && m.AppStatus.Status["Stage"] == cStage
            )));
        }
    }
}
