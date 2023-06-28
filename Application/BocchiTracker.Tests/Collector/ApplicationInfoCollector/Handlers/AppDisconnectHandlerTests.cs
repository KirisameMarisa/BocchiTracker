using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ModelEventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector.Handlers
{
    public class AppDisconnectHandlerTests
    {
        private AppStatusBundles _bundles;
        private AppDisconnectHandler _handler;

        public AppDisconnectHandlerTests()
        {
            _bundles = new AppStatusBundles();
            _handler = new AppDisconnectHandler(_bundles);
        }

        [Fact]
        public async Task Test_RemoveElemFromAppStatusBundles_UsingHandler()
        {
            // Arrange
            const int cClientID = 2;

            _bundles.Add(cClientID);
            var request = new AppDisconnectEvent { ClientID = cClientID };

            // Act
            await _handler.Handle(request, default);

            // Assert
            Assert.False(_bundles.Bundles.ContainsKey(2));
        }
    }
}
