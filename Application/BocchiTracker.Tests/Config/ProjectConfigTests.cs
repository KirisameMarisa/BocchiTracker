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
        public void GetServiceURL_ServiceURLExists_ReturnsURL()
        {
            // Arrange
            var config = new ProjectConfig
            {
                ServiceURLs = new List<Dictionary<ServiceDefinitions, string>>
                {
                    new Dictionary<ServiceDefinitions, string>
                    {
                        { ServiceDefinitions.Github, "https://service1.example.com" },
                        { ServiceDefinitions.Redmine, "https://service2.example.com" }
                    }
                }
            };

            // Act
            var url1 = config.GetServiceURL(ServiceDefinitions.Github);
            var url2 = config.GetServiceURL(ServiceDefinitions.Redmine);
            var url3 = config.GetServiceURL(ServiceDefinitions.Slack);

            // Assert
            Assert.Equal("https://service1.example.com", url1);
            Assert.Equal("https://service2.example.com", url2);
            Assert.Null(url3);
        }

        [Fact]
        public void GetServiceURL_NoServiceURL_ReturnsNull()
        {
            // Arrange
            var config = new ProjectConfig();

            // Act
            var url = config.GetServiceURL(ServiceDefinitions.Github);

            // Assert
            Assert.Null(url);
        }
    }
}
