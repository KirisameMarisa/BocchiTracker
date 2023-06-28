using BocchiTracker.ProcessLink.ProcessData;
using Google.FlatBuffers;
using MediatR;
using Moq;
using BocchiTracker.ProcessLinkQuery.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataAppBasicInfoTests
    {
        [Fact]
        public async Task Test_Handle()
        {
            const int cClientID = 9999;

            var mediatorMock = new Mock<IMediator> { CallBase = true };

            var fbb = new FlatBufferBuilder(1024);
            var app_pid = 10009;
            const string cAppName = "xUnitTest";
            var app_name = fbb.CreateString(cAppName);
            const string cArgs = "xUnitTest args";
            var app_args = fbb.CreateString(cArgs);
            const string cPlatform = "Windows";
            var app_platform = fbb.CreateString(cPlatform);

            AppBasicInfo.StartAppBasicInfo(fbb);
            AppBasicInfo.AddPid(fbb, app_pid);
            AppBasicInfo.AddAppName(fbb, app_name);
            AppBasicInfo.AddArgs(fbb, app_args);
            AppBasicInfo.AddPlatform(fbb, app_platform);
            var table = AppBasicInfo.EndAppBasicInfo(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, QueryID.AppBasicInfo);
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

            Assert.Equal((byte)QueryID.AppBasicInfo, captured?.QueryID);
            Assert.Equal(cClientID, captured?.ClientID);

            var status = captured?.Status as Dictionary<string, dynamic>;
            Assert.NotNull(status);

            Assert.Equal(app_pid, (long)status["Pid"]);
            Assert.Equal(cAppName, (string)status["AppName"]);
            Assert.Equal(cArgs, (string)status["Args"]);
            Assert.Equal(cPlatform, (string)status["Platform"]);
        }
    }
}
