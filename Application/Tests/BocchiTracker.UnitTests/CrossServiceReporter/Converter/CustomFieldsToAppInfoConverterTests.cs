using System.Collections.Generic;
using System.Threading.Tasks;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter.Converter;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientData;
using Moq;
using Xunit;

namespace BocchiTracker.BocchiTracker.UnitTests.CrossServiceReporter.Converter
{
    public class CustomFieldsToAppInfoConverterTests
    {
        [Fact]
        public async Task Convert_ReturnsCorrectCustomFields()
        {
            // Arrange
            var serviceType = ServiceDefinitions.Redmine;
            var mockDataRepository = new Mock<IDataRepository>();
            var expectedData = new List<IdentifierData>
            {
                new IdentifierData { Id = "Id1", Name = "CustomField1" },
                new IdentifierData { Id = "Id2", Name = "CustomField3" }
            };
            mockDataRepository.Setup(repo => repo.GetCustomFields(serviceType)).ReturnsAsync(expectedData);

            var customFieldLists = new Dictionary<string, List<string>>
            {
                { "CustomField1", new List<string> { "Value1", "Value2" } },
                { "CustomField2", new List<string> { "Value3" } }
            };
            var metaListService = new CustomFieldListService();

            // Act
            await metaListService.Load(mockDataRepository.Object);

            var converter = new CustomFieldsToAppInfoConverter();
            var result = converter.Convert(serviceType, customFieldLists, metaListService);

            // Assert
            Assert.Single(result); // CustomField1 のみが期待される
            Assert.True(result.ContainsKey("Id1"));
            Assert.Equal(2, result["Id1"].Count);
            Assert.Contains("Value1", result["Id1"]);
            Assert.Contains("Value2", result["Id1"]);
        }
    }
}