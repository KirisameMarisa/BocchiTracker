using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.CrossServiceReporter.CreateTicketData;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateTicketTypeTests
    {
        [Fact]
        public void Create_ShouldReturnTicketType_WhenMappingExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { TicketType = "Bug" };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new Dictionary<string, string>
            {
                { "Bug", "Defect" },
                { "Feature", "Enhancement" }
            };

            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("Defect", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMappingDoesNotExist()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { TicketType = "Task" };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new Dictionary<string, string>
            {
                { "Bug", "Defect" },
                { "Feature", "Enhancement" }
            };

            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenTicketTypeIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { TicketType = null };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new Dictionary<string, string>
            {
                { "Bug", "Defect" },
                { "Feature", "Enhancement" }
            };

            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
