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
using System.Collections.ObjectModel;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateLabelsTests
    {
        [Fact]
        public void Create_ShouldReturnLabels_WhenTicketDataExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData
            {
                Lables = new List<string> { "Label1", "Label2", "Label3" }
            };

            var inConfig = new ServiceConfig();

            var createLabels = new CreateLabels();

            // Act
            var result = createLabels.Create(inService, inBundle, inConfig);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("Label1", result);
            Assert.Contains("Label2", result);
            Assert.Contains("Label3", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenTicketDataIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            var inConfig = new ServiceConfig();

            var createLabels = new CreateLabels();

            // Act
            var result = createLabels.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
