using Xunit;
using Moq;
using System.Collections.Generic;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using System.Threading.Tasks;
using Prism.Events;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector.Handlers
{
    public class AppStatusQueryHandlerTests
    {
        private AppStatusBundles _bundles;

        public AppStatusQueryHandlerTests()
        {
            _bundles = new AppStatusBundles();
        }

        [Fact]
        public void Test_Handle_ShouldCallProcessAppBasicInfo_WhenQueryIdIs0()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<AppStatusQueryEvent>())
                .Returns(new AppStatusQueryEvent());
            var handler = new AppStatusQueryHandler(mockEvent.Object, _bundles);
            var request = new AppStatusQueryEventParameter(new AppStatus
            {
                ClientID = 1,
                QueryID = (byte)QueryID.AppBasicInfo,
                Status = new Dictionary<string, string>
                {
                    { "AppBasicInfo.pid", "1234" },
                    { "AppBasicInfo.app_name", "TestApp" },
                    { "AppBasicInfo.args", "TestArgs" },
                    { "AppBasicInfo.platform", "Windows" },
                },
            });

            // Act
            handler.Handle(request);

            // Assert
            var asset_bundle = _bundles.GetBundlesByClientID(1);
            Assert.NotNull(asset_bundle);

            Assert.Equal("1234", asset_bundle.AppBasicInfo.Pid);
            Assert.Equal("TestApp", asset_bundle.AppBasicInfo.AppName);
            Assert.Equal("TestArgs", asset_bundle.AppBasicInfo.Args);
            Assert.Equal("Windows", asset_bundle.AppBasicInfo.Platform);
        }

        [Fact]
        public void Test_Handle_ShouldCallProcessAppStatusDynamic_WhenQueryIdIsNot0()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<AppStatusQueryEvent>())
                .Returns(new AppStatusQueryEvent());
            var handler = new AppStatusQueryHandler(mockEvent.Object, _bundles);

            var request = new AppStatusQueryEventParameter(new AppStatus
            {
                ClientID = 2,
                QueryID = 0,
                Status = new Dictionary<string, string>
                {
                    {"Memory", "4GB"},
                    {"CPU", "2.4 GHz"}
                }
            });

            // Act
            handler.Handle(request);

            // Assert
            var asset_bundle = _bundles.GetBundlesByClientID(2);
            Assert.NotNull(asset_bundle);

            Assert.True(asset_bundle.AppStatusDynamics.ContainsKey("Memory"));
            Assert.Equal("4GB", (string)asset_bundle.AppStatusDynamics["Memory"]);
            Assert.True(asset_bundle.AppStatusDynamics.ContainsKey("CPU"));
            Assert.Equal("2.4 GHz", (string)asset_bundle.AppStatusDynamics["CPU"]);
        }
    }
}