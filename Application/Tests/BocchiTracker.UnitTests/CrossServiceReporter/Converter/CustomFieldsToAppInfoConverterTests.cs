using System.Collections.Generic;
using System.Threading.Tasks;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.CrossServiceReporter.Converter;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientData;
using Moq;
using Xunit;

namespace BocchiTracker.Tests.CrossServiceReporter.Converter
{
    public class CustomFieldsToAppInfoConverterTests
    {
        [Fact]
        public void Convert_ReturnsCorrectCustomFields()
        {
            // Arrange
            var serviceConfig = new ServiceConfig { 
                QueryFieldMappings = new List<ValueMapping>
                {
                    new ValueMapping { Definition = "Id1", Name = "CustomField1" },
                    new ValueMapping { Definition = "Id2", Name = "CustomField3" }
                }
            };

            var customFieldLists = new Dictionary<string, List<string>>
            {
                { "CustomField1", new List<string> { "Value1", "Value2" } },
                { "CustomField2", new List<string> { "Value3" } }
            };

            // Act
            var converter = new CustomFieldsToAppInfoConverter();
            var result = converter.Convert(serviceConfig, customFieldLists);

            // Assert
            Assert.Single(result); // CustomField1 のみが期待される
            Assert.True(result.ContainsKey("Id1"));
            Assert.Equal(2, result["Id1"].Count);
            Assert.Contains("Value1", result["Id1"]);
            Assert.Contains("Value2", result["Id1"]);
        }
    }
}