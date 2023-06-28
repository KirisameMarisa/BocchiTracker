using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientAdapters.Controllers;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ProjectConfig;

namespace BocchiTracker.Tests.ServiceClientAdapters.Controllers
{
    public class ServiceControllerTests
    {
        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var mockFactory = new Mock<IServiceClientAdapterFactory>();
            var mockAdapter = new Mock<IServiceClientAdapter>();

            var authConfig = new AuthConfig { Username = "testuser", Password = "testpass" };

            mockAdapter.Setup(a => a.Authenticate(authConfig, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            mockFactory.Setup(f => f.CreateServiceClientAdapter(It.IsAny<ServiceDefinitions>())).Returns(mockAdapter.Object);

            var controller = new AuthenticationController(mockFactory.Object);

            // Act
            var result = await controller.Authenticate(ServiceDefinitions.Github, authConfig, "https://api.github.com", null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Post_ValidTicketData_CallsPostOnService()
        {
            // Arrange
            var mockFactory = new Mock<IServiceClientAdapterFactory>();
            var mockAdapter = new Mock<IServiceClientAdapter>();

            var ticketData = new TicketData { /* populate properties here */ };

            mockFactory.Setup(f => f.CreateServiceClientAdapter(It.IsAny<ServiceDefinitions>())).Returns(mockAdapter.Object);

            var controller = new TicketPostingController(mockFactory.Object);

            // Act
            controller.Post(ServiceDefinitions.Github, ticketData);

            // Assert
            mockAdapter.Verify(a => a.Post(ticketData), Times.Once);
        }
    }
}
