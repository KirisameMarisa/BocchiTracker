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
    public class CreateCustomfieldsTests
    {
        [Fact]
        public void Create_ShouldReturnCustomFields_WhenConfigAndTicketDataExist()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var customFields = new Dictionary<string, List<string>>
            {
                { "Field1", new List<string> { "Value1", "Value2" } },
                { "Field2", new List<string> { "Value3" } }
            };

            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { CustomFields = customFields };

            var inConfig = new ServiceConfig
            {
                QueryFieldMappings = new List<ValueMapping>
                {
                   new ValueMapping { Definition = "Field1",    Name = "CustomField1" },
                   new ValueMapping { Definition = "Field2",     Name = "CustomField2" }
                }
            };

            var createCustomFields = new CreateCustomfields();

            // Act
            var result = createCustomFields.Create(inService, inBundle, inConfig);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("CustomField1"));
            Assert.True(result.ContainsKey("CustomField2"));
            Assert.Equal(customFields["Field1"], result["CustomField1"]);
            Assert.Equal(customFields["Field2"], result["CustomField2"]);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenConfigIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var customFields = new Dictionary<string, List<string>>
            {
                { "Field1", new List<string> { "Value1", "Value2" } },
                { "Field2", new List<string> { "Value3" } }
            };

            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { CustomFields = customFields };

            var inConfig = new ServiceConfig();

            var createCustomFields = new CreateCustomfields();

            // Act
            var result = createCustomFields.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenTicketDataIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            var inConfig = new ServiceConfig
            {
                QueryFieldMappings = new List<ValueMapping>
                {
                   new ValueMapping { Definition = "Field1",    Name = "CustomField1" },
                   new ValueMapping { Definition = "Field2",     Name = "CustomField2" }
                }
            };

            var createCustomFields = new CreateCustomfields();

            // Act
            var result = createCustomFields.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
