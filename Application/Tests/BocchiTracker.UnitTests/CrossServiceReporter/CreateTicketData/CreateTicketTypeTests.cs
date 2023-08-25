using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.CrossServiceReporter.CreateTicketData;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using BocchiTracker.ModelEvent;
using Prism.Events;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateTicketTypeTests
    {
        [Fact]
        public async Task Create_ShouldReturnTicketType_WhenMappingExists()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var tickets = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "Defect" },
                new IdentifierData { Id = "2", Name = "Enhancement" },
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetTicketTypes(inService)).ReturnsAsync(tickets);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData { TicketType = "Bug" };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "Bug",      Name = "Defect" },
                new ValueMapping { Definition = "Feature",  Name = "Enhancement" }
            };
     
            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenMappingDoesNotExist()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var tickets = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "Defect" },
                new IdentifierData { Id = "2", Name = "Enhancement" },
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetTicketTypes(inService)).ReturnsAsync(tickets);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData { TicketType = "Task" };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "Bug",      Name = "Defect" },
                new ValueMapping { Definition = "Feature",  Name = "Enhancement" }
            };

            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenTicketTypeIsNull()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var tickets = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "Defect" },
                new IdentifierData { Id = "2", Name = "Enhancement" },
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetTicketTypes(inService)).ReturnsAsync(tickets);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData { TicketType = null };

            var inConfig = new ServiceConfig();
            inConfig.TicketTypeMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "Bug",      Name = "Defect" },
                new ValueMapping { Definition = "Feature",  Name = "Enhancement" }
            };

            var createTicketType = new CreateTicketType();

            // Act
            var result = createTicketType.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
