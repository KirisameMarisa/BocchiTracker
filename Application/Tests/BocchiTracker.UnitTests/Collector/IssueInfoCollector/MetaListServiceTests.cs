using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientAdapters.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Collector.IssueInfoCollector
{
    public class MetaListServiceTests
    {
        [Fact]
        public async Task Load_ShouldPopulateData()
        {
            // Arrange
            var mockDataRepository = new Mock<IDataRepository>();
            var serviceType = ServiceDefinitions.Redmine;
            var expectedData = new List<IdentifierData> { new IdentifierData() };
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType)).ReturnsAsync(expectedData);

            var metaListService = new LabelListService();

            // Act
            await metaListService.Load(mockDataRepository.Object);
            var actualData = metaListService.GetData(serviceType);

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetUnifiedData_ShouldReturnCombinedData()
        {
            // Arrange
            var serviceType1 = ServiceDefinitions.Redmine;
            var serviceType2 = ServiceDefinitions.JIRA;
            var data1 = new List<IdentifierData> { new IdentifierData() };
            var data2 = new List<IdentifierData> { new IdentifierData() };
            var expectedUnifiedData = new List<IdentifierData> { new IdentifierData() };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType1)).ReturnsAsync(data1);
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType2)).ReturnsAsync(data2);

            var metaListService = new LabelListService();
            await metaListService.Load(mockDataRepository.Object);

            // Act
            var unifiedData = metaListService.GetUnifiedData();

            // Assert
            Assert.Equal(expectedUnifiedData, unifiedData);
        }
    }

    public class UserListServiceTests
    {
        [Fact]
        public async Task Load_ShouldPopulateData()
        {
            // Arrange
            var mockDataRepository = new Mock<IDataRepository>();
            var serviceType = ServiceDefinitions.Redmine;
            var expectedData = new List<UserData> { new UserData() };
            mockDataRepository.Setup(repo => repo.GetUsers(serviceType)).ReturnsAsync(expectedData);

            var service = new UserListService();

            // Act
            await service.Load(mockDataRepository.Object);
            var actualData = service.GetData(serviceType);

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetUnifiedData_ShouldReturnCombinedData()
        {
            // Arrange
            var serviceType1 = ServiceDefinitions.Redmine;
            var serviceType2 = ServiceDefinitions.JIRA;
            var data1 = new List<UserData> { new UserData() };
            var data2 = new List<UserData> { new UserData() };
            var expectedUnifiedData = new List<UserData> { new UserData() };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetUsers(serviceType1)).ReturnsAsync(data1);
            mockDataRepository.Setup(repo => repo.GetUsers(serviceType2)).ReturnsAsync(data2);

            var service = new UserListService();
            await service.Load(mockDataRepository.Object);

            // Act
            var unifiedData = service.GetUnifiedData();

            // Assert
            Assert.Equal(expectedUnifiedData, unifiedData);
        }
    }

    public class TicketTypeListServiceTests
    {
        [Fact]
        public async Task Load_ShouldPopulateData()
        {
            // Arrange
            var mockDataRepository = new Mock<IDataRepository>();
            var serviceType = ServiceDefinitions.Redmine;
            var expectedData = new List<IdentifierData> { new IdentifierData() };
            mockDataRepository.Setup(repo => repo.GetTicketTypes(serviceType)).ReturnsAsync(expectedData);

            var service = new TicketTypeListService();

            // Act
            await service.Load(mockDataRepository.Object);
            var actualData = service.GetData(serviceType);

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetUnifiedData_ShouldReturnCombinedData()
        {
            // Arrange
            var serviceType1 = ServiceDefinitions.Redmine;
            var serviceType2 = ServiceDefinitions.JIRA;
            var data1 = new List<IdentifierData> { new IdentifierData() };
            var data2 = new List<IdentifierData> { new IdentifierData() };
            var expectedUnifiedData = new List<IdentifierData> { new IdentifierData() };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetTicketTypes(serviceType1)).ReturnsAsync(data1);
            mockDataRepository.Setup(repo => repo.GetTicketTypes(serviceType2)).ReturnsAsync(data2);

            var service = new TicketTypeListService();
            await service.Load(mockDataRepository.Object);

            // Act
            var unifiedData = service.GetUnifiedData();

            // Assert
            Assert.Equal(expectedUnifiedData, unifiedData);
        }
    }

    public class PriorityListServiceTests
    {
        [Fact]
        public async Task Load_ShouldPopulateData()
        {
            // Arrange
            var mockDataRepository = new Mock<IDataRepository>();
            var serviceType = ServiceDefinitions.Redmine;
            var expectedData = new List<IdentifierData> { new IdentifierData() };
            mockDataRepository.Setup(repo => repo.GetPriorities(serviceType)).ReturnsAsync(expectedData);

            var service = new PriorityListService();

            // Act
            await service.Load(mockDataRepository.Object);
            var actualData = service.GetData(serviceType);

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetUnifiedData_ShouldReturnCombinedData()
        {
            // Arrange
            var serviceType1 = ServiceDefinitions.Redmine;
            var serviceType2 = ServiceDefinitions.JIRA;
            var data1 = new List<IdentifierData> { new IdentifierData() };
            var data2 = new List<IdentifierData> { new IdentifierData() };
            var expectedUnifiedData = new List<IdentifierData> { new IdentifierData() };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetPriorities(serviceType1)).ReturnsAsync(data1);
            mockDataRepository.Setup(repo => repo.GetPriorities(serviceType2)).ReturnsAsync(data2);

            var service = new PriorityListService();
            await service.Load(mockDataRepository.Object);

            // Act
            var unifiedData = service.GetUnifiedData();

            // Assert
            Assert.Equal(expectedUnifiedData, unifiedData);
        }
    }

    public class LabelListServiceTests
    {
        [Fact]
        public async Task Load_ShouldPopulateData()
        {
            // Arrange
            var mockDataRepository = new Mock<IDataRepository>();
            var serviceType = ServiceDefinitions.Redmine;
            var expectedData = new List<IdentifierData> { new IdentifierData() };
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType)).ReturnsAsync(expectedData);

            var service = new LabelListService();

            // Act
            await service.Load(mockDataRepository.Object);
            var actualData = service.GetData(serviceType);

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetUnifiedData_ShouldReturnCombinedData()
        {
            // Arrange
            var serviceType1 = ServiceDefinitions.Redmine;
            var serviceType2 = ServiceDefinitions.JIRA;
            var data1 = new List<IdentifierData> { new IdentifierData() };
            var data2 = new List<IdentifierData> { new IdentifierData() };
            var expectedUnifiedData = new List<IdentifierData> { new IdentifierData() };

            var mockDataRepository = new Mock<IDataRepository>();
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType1)).ReturnsAsync(data1);
            mockDataRepository.Setup(repo => repo.GetLabels(serviceType2)).ReturnsAsync(data2);

            var service = new LabelListService();
            await service.Load(mockDataRepository.Object);

            // Act
            var unifiedData = service.GetUnifiedData();

            // Assert
            Assert.Equal(expectedUnifiedData, unifiedData);
        }
    }
}
