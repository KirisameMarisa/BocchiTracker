﻿using BocchiTracker.Config.Configs;
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
    public class CreatePriorityTests
    {
        [Fact]
        public void Create_ShouldReturnPriority_WhenMappingExists()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = "High" };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition =  "High",    Name = "P1" },
                new ValueMapping { Definition =  "Medium",  Name = "P2" },
                new ValueMapping { Definition =  "Low",     Name = "P3" }
            };
  
            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Equal("P1", result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMappingDoesNotExist()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = "Urgent" };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "High",     Name = "P1" },
                new ValueMapping { Definition = "Medium",   Name = "P2" },
                new ValueMapping { Definition = "Low",      Name = "P3" }
            };

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenPriorityIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = null };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = new List<ValueMapping>
            {
                new ValueMapping { Definition = "High",     Name = "P1" },
                new ValueMapping { Definition = "Medium",   Name = "P2" },
                new ValueMapping { Definition = "Low",      Name = "P3" }
            };

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Create_ShouldReturnNull_WhenMappingIsNull()
        {
            // Arrange
            var inService = ServiceDefinitions.Redmine;
            var inBundle = new IssueInfoBundle();
            inBundle.TicketData = new TicketData { Priority = "High" };

            var inConfig = new ServiceConfig();
            inConfig.PriorityMappings = null;

            var createPriority = new CreatePriority();

            // Act
            var result = createPriority.Create(inService, inBundle, inConfig);

            // Assert
            Assert.Null(result);
        }
    }
}