using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ModelEvent;
using Moq;
using Prism.Events;
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

        public AppDisconnectHandlerTests()
        {
            _bundles = new AppStatusBundles();
        }

        [Fact]
        public void Test_RemoveElemFromAppStatusBundles_UsingHandler()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<AppDisconnectEvent>())
                .Returns(new AppDisconnectEvent());
            
            var handler = new AppDisconnectHandler(mockEvent.Object, _bundles);
            const int cClientID = 2;

            _bundles.Add(cClientID, new Dictionary<string, string>());
            var request = new AppDisconnectEventParameter(cClientID);

            // Act
            handler.Handle(request);

            // Assert
            Assert.False(_bundles.Bundles.ContainsKey(2));
        }
    }
}
