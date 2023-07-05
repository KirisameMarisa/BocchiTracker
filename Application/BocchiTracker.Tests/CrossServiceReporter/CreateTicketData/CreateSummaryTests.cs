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
    public class CreateSummaryTests
    {
        [Fact]
        public void Create_ShouldReturnSummary_WhenSummaryExists()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Summary = "Sample Summary" };

            var inConfig = new ServiceConfig();

            var createSummary = new CreateSummary();

            // Act
            var result = createSummary.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("Sample Summary", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenSummaryIsNull()
        {
            // Arrange
            var inService = IssueServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Summary = null };

            var inConfig = new ServiceConfig();

            var createSummary = new CreateSummary();

            // Act
            var result = createSummary.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}
