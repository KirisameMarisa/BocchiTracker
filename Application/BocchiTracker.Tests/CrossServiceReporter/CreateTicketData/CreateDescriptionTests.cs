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
using System.Collections.ObjectModel;

namespace BocchiTracker.Tests.CrossServiceReporter.CreateTicketData
{
    public class CreateDescriptionTests
    {
        [Fact]
        public void Create_ShouldReturnDescription_WhenConfigFormatExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData
            {
                Summary = "Ticket Summary",
                Description = "Ticket Description",
                Assignee = "John",
                Lables = new List<string> { "Label1", "Label2" },
                Priority = "High",
                CustomFields = new Dictionary<string, List<string>>
                {
                    { "Field1", new List<string> { "Value1", "Value2" } },
                    { "Field2", new List<string> { "Value3" } }
                }
            };

            var inConfig = new ServiceConfig
            {
                DescriptionFormat = "Summary: {Summary}\nDescription: {Description}\nAssignee: {Assignee}\nLabels: {Lables}\nPriority: {Priority}\nField1: {Field1}\nField2: {Field2}"
            };

            var createDescription = new CreateDescription();

            // Act
            var result = createDescription.Create(inService, inBundle, inConfig);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Summary: Ticket Summary", result);
            Assert.Contains("Description: Ticket Description", result);
            Assert.Contains("Assignee: John", result);
            Assert.Contains("Labels: Label1, Label2", result);
            Assert.Contains("Priority: High", result);
            Assert.Contains("Field1: Value1, Value2", result);
            Assert.Contains("Field2: Value3", result);
        }

        [Fact]
        public void Create_ShouldReturnDescription_WhenConfigFormatIsEmpty()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData
            {
                Summary = "Ticket Summary",
                Description = "Ticket Description",
                Assignee = "John",
                Lables = new List<string> { "Label1", "Label2" },
                Priority = "High",
                CustomFields = new Dictionary<string, List<string>>
                {
                    { "Field1", new List<string> { "Value1", "Value2" } },
                    { "Field2", new List<string> { "Value3" } }
                }
            };

            var inConfig = new ServiceConfig
            {
                DescriptionFormat = ""
            };

            var createDescription = new CreateDescription();

            // Act
            var result = createDescription.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("Ticket Description", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenBundleIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            var inConfig = new ServiceConfig
            {
                DescriptionFormat = "Summary: {Summary}\nDescription: {Description}\nAssignee: {Assignee}\nLabels: {Lables}\nPriority: {Priority}\nField1: {Field1}\nField2: {Field2}"
            };

            var createDescription = new CreateDescription();

            // Act
            var result = createDescription.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal(result, inConfig.DescriptionFormat);
        }
    }
}
