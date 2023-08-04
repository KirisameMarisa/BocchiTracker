using BocchiTracker.ProcessLink.ProcessData;
using Google.FlatBuffers;
using Moq;
using BocchiTracker.ProcessLinkQuery.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Prism.Events;
using BocchiTracker.ModelEvent;
using BocchiTracker.ApplicationInfoCollector.Handlers;

namespace BocchiTracker.Tests.ProcessLink.ProcessData
{
    public class ProcessDataAppBasicInfoTests
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
            serviceProcessData.Register(QueryID.AppBasicInfo, new ProcessDataAppBasicInfo());

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
            serviceProcessData.Process(eventAggregatorMock.Object, cClientID, Packet.GetRootAsPacket(buffer));

            // Assert
            mockedEvent.Verify(x => x.Publish(It.Is<AppStatusQueryEventParameter>(
                   m => m.AppStatus.ClientID == cClientID
                && m.AppStatus.QueryID == (byte)QueryID.AppBasicInfo
                && m.AppStatus.Status["AppBasicInfo.pid"] == app_pid.ToString()
                && m.AppStatus.Status["AppBasicInfo.app_name"] == cAppName
                && m.AppStatus.Status["AppBasicInfo.args"] == cArgs
                && m.AppStatus.Status["AppBasicInfo.platform"] == cPlatform
            )));
        }
    }
}
