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
using Moq;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateWatchUserTests
    {
        [Fact]
        public async Task Create_ShouldReturnWatchUserIds_WhenWatchersExist()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John" },
                new UserData { Id = "2", Name = "Jane" }
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Watcheres = new List<string> { "John", "Jane" } };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("1", result);
            Assert.Contains("2", result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenWatchersDoNotExist()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John" },
                new UserData { Id = "2", Name = "Jane" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Watcheres = new List<string> { "Erecto", "Tom" } };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenUsersAreNotLoaded()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;

            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Watcheres = new List<string> { "John", "Jane" } };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenWatchersAreNull()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John" },
                new UserData { Id = "2", Name = "Jane" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object);
            inBundle.TicketData = new TicketData { Watcheres = null };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
