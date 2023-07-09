using Xunit;
using Moq;
using System.Collections.Generic;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ModelEventBus;
using BocchiTracker.ProcessLinkQuery.Queries;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector.Handlers
{
    public class AppStatusQueryHandlerTests
    {
        private AppStatusBundles _bundles;
        private AppStatusQueryHandler _handler;

        public AppStatusQueryHandlerTests()
        {
            _bundles = new AppStatusBundles();
            _handler = new AppStatusQueryHandler(_bundles);
        }

        [Fact]
        public async Task Test_Handle_ShouldCallProcessAppBasicInfo_WhenQueryIdIs0()
        {
            // Arrange
            var request = new AppStatusQueryEvent(new AppStatus
            {
                ClientID = 1,
                QueryID = (byte)QueryID.AppBasicInfo,
                Status = new Dictionary<string, string>
                {
                    { "Pid", "1234" },
                    { "AppName", "TestApp" },
                    { "Args", "TestArgs" },
                    { "Platform", "Windows" },
                },
            });

            // Act
            await _handler.Handle(request, default);

            // Assert
            var asset_bundle = _bundles.GetBundlesByClientID(1);
            Assert.NotNull(asset_bundle);

            Assert.Equal("1234", asset_bundle.AppBasicInfo.Pid);
            Assert.Equal("TestApp", asset_bundle.AppBasicInfo.AppName);
            Assert.Equal("TestArgs", asset_bundle.AppBasicInfo.Args);
            Assert.Equal("Windows", asset_bundle.AppBasicInfo.Platform);
        }

        [Fact]
        public async Task Test_Handle_ShouldCallProcessAppStatusDynamic_WhenQueryIdIsNot0()
        {
            // Arrange
            var request = new AppStatusQueryEvent(new AppStatus
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
            await _handler.Handle(request, default);

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