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
using System.Collections.ObjectModel;
using Redmine.Net.Api.Types;
using BocchiTracker.ModelEvent;
using Prism.Events;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateWatchUserTests
    {
        [Fact]
        public async Task Create_ShouldReturnWatchUserIds_WhenWatchersExist()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Email = "John@exampl.com" },
                new UserData { Id = "2", Email = "Jane@exampl.com" }
            };
            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData { Watchers = new List<UserData> { 
                    new UserData { Name = "John", Email = "John@exampl.com" }, 
                    new UserData { Name = "Jane", Email = "Jane@exampl.com" } 
                } 
            };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("1", result[0].Id);
            Assert.Contains("2", result[1].Id);
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenWatchersDoNotExist()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John", Email = "John@exampl.com" },
                new UserData { Id = "2", Name = "Jane", Email = "Jane@exampl.com" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData {
                Watchers = new List<UserData> {
                    new UserData { Name = "Erecto", Email = "Erecto@exampl.com" },
                    new UserData { Name = "Tom",    Email = "Tom@exampl.com" }
                }
            };
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
            var inService = ServiceDefinitions.Redmine;

            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData {
                Watchers = new List<UserData> {
                    new UserData { Name = "John", Email = "John@exampl.com" },
                    new UserData { Name = "Jane", Email = "Jane@exampl.com" }
                }
            };

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
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ProgressingEvent>())
                .Returns(new ProgressingEvent());
            var inService = ServiceDefinitions.Redmine;
            var users = new List<UserData>
            {
                new UserData { Id = "1", Name = "John" },
                new UserData { Id = "2", Name = "Jane" }
            };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(inService)).ReturnsAsync(users);

            var inBundle = new IssueInfoBundle();
            await inBundle.Initialize(mockDataRepository.Object, mockEvent.Object);
            inBundle.TicketData = new TicketData { Watchers = null };

            var inConfig = new ServiceConfig();

            var createWatchUser = new CreateWatchUser();

            // Act
            var result = createWatchUser.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
