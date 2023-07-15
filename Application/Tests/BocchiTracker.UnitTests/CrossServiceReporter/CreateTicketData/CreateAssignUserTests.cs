using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.CrossServiceReporter.CreateTicketData;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateAssignUserTests
    {
        [Fact]
        public async Task Create_ShouldReturnUserId_WhenAssigneeExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John" },
                new UserData { Id = "2", Name = "Jane" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Assign = "John" };

            var createAssignUser = new CreateAssignUser();

            // Act
            var result = createAssignUser.Create(inService, inBundle, new ServiceConfig());

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenAssigneeDoesNotExist()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "Jane" },
                new UserData { Id = "2", Name = "Alice" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Assign = "John" };

            var createAssignUser = new CreateAssignUser();

            // Act
            var result = createAssignUser.Create(inService, inBundle, new ServiceConfig());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenUserListIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var mockDataRepository = new Mock<IDataRepository>();

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Assign = "John" };

            var createAssignUser = new CreateAssignUser();

            // Act
            var result = createAssignUser.Create(inService, inBundle, new ServiceConfig());

            // Assert
            Assert.Null(result);
        }
    }
}
