using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Config
{
    public class ProjectConfigTests
    {
        [Fact]
        public void GetServiceConfig_ServiceURLExists_ReturnsURL()
        {
            // Arrange
            var config = new ProjectConfig
            {
                ServiceConfigs = new List<ServiceConfig>
                {
                    new ServiceConfig { Service = IssueServiceDefinitions.Github,    URL = "https://service1.example.com" },
                    new ServiceConfig { Service = IssueServiceDefinitions.Redmine,   URL = "https://service2.example.com" },
                }
            };

            // Act
            var config1 = config.GetServiceConfig(IssueServiceDefinitions.Github);
            var config2 = config.GetServiceConfig(IssueServiceDefinitions.Redmine);
            var config3 = config.GetServiceConfig(IssueServiceDefinitions.Slack);

            // Assert
            Assert.NotNull(config1);
            Assert.NotNull(config2);
            Assert.Null(config3);
            Assert.Equal("https://service1.example.com", config1.URL);
            Assert.Equal("https://service2.example.com", config2.URL);
        }

        [Fact]
        public void GetServiceConfig_ReturnsNull()
        {
            // Arrange
            var config = new ProjectConfig();

            // Act
            var service_config = config.GetServiceConfig(IssueServiceDefinitions.Github);

            // Assert
            Assert.Null(service_config);
        }
    }
}
