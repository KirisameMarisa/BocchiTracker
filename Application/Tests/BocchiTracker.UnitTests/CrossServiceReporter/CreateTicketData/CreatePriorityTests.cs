using BocchiTracker.ServiceClientData.Configs;
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

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreatePriorityTests
    {
        [Fact]
        public async Task Create_ShouldReturnPriority_WhenMappingExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var users = new List<IdentifierData>
            {
                new IdentifierData { Id = "1", Name = "P1"  },
                new IdentifierData { Id = "2", Name = "P2"  },
                new IdentifierData { Id = "3", Name = "P3"  },
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetPriorities(inService)).ReturnsAsync(users);
            
            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Priority = "High" };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition =  "High",    Name = "P1" },
                new ValueMapping { Definition =  "Medium",  Name = "P2" },
                new ValueMapping { Definition =  "Low",     Name = "P3" }
            };
  
            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMappingDoesNotExist()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = "Urgent" };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "High",     Name = "P1" },
                new ValueMapping { Definition = "Medium",   Name = "P2" },
                new ValueMapping { Definition = "Low",      Name = "P3" }
            };

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenPriorityIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = null };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "High",     Name = "P1" },
                new ValueMapping { Definition = "Medium",   Name = "P2" },
                new ValueMapping { Definition = "Low",      Name = "P3" }
            };

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMappingIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = "High" };

            var inConfig = new ServiceConfig();

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
