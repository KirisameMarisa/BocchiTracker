using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientData.Configs;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Config
{
    public class ProjectConfigRepositoryTests
    {
        [Fact]
        public void Load_ValidConfig_ReturnsConfigObject()
        {
            // Arrange
            string filepath = "config.yaml";
            var fileSystemMock = new Mock<IFileSystem>();
            var configRepository = new ConfigRepository<ProjectConfig>(fileSystemMock.Object);
            configRepository.SetLoadFilename(filepath);
            var config = new ProjectConfig
            {
                ServiceConfigs = new List<ServiceConfig>
                {
                    new ServiceConfig { Service = ServiceDefinitions.Github,    URL = "https://service1.example.com" },
                    new ServiceConfig { Service = ServiceDefinitions.Redmine,   URL = "https://service2.example.com" },
                }
            };

            var yaml = @"
ServiceConfigs:
    - Service: Github
      URL: https://service1.example.com
    - Service: Redmine
      URL: https://service2.example.com";

            using var memory_stream = new MemoryStream();
            using var writer = new StreamWriter(memory_stream);
            writer.WriteLine(yaml);
            writer.Flush();
            memory_stream.Position = 0;

            fileSystemMock
                .Setup(x => x.File.OpenText(filepath))
                .Returns(new StreamReader(memory_stream));

            // Act
            var result = configRepository.Load();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(config.ServiceConfigs.Count, result.ServiceConfigs.Count);
            Assert.Equal(config.ServiceConfigs[0].Service,  result.ServiceConfigs[0].Service);
            Assert.Equal(config.ServiceConfigs[0].URL,      result.ServiceConfigs[0].URL);

            Assert.Equal(config.ServiceConfigs[1].Service,  result.ServiceConfigs[1].Service);
            Assert.Equal(config.ServiceConfigs[1].URL,      result.ServiceConfigs[1].URL);
        }

        [Fact]
        public void Load_InvalidConfig_ReturnsNull()
        {
            // Arrange
            string filepath = "config.yaml";
            var fileSystemMock = new Mock<IFileSystem>();
            var configRepository = new ConfigRepository<ProjectConfig>(fileSystemMock.Object);
            configRepository.SetLoadFilename(filepath);
            var yaml = "Invalid YAML";

            using var memory_stream = new MemoryStream();
            using var writer = new StreamWriter(memory_stream);
            writer.WriteLine(yaml);
            writer.Flush();
            memory_stream.Position = 0;

            fileSystemMock
                .Setup(x => x.File.OpenText(filepath))
                .Returns(new StreamReader(memory_stream));

            // Act
            ProjectConfig? outConfig;
            var result = configRepository.TryLoad(out outConfig);

            // Assert
            Assert.Null(outConfig);
            Assert.False(result);
        }

        [Fact]
        public void Save_Config_SavesToFile()
        {
            // Arrange
            string filepath = "config.yaml";
            var fileSystemMock = new Mock<IFileSystem>();
            var configRepository = new ConfigRepository<ProjectConfig>(fileSystemMock.Object);
            configRepository.SetLoadFilename(filepath);
            var config = new ProjectConfig
            {
                ServiceConfigs = new List<ServiceConfig>
                {
                    new ServiceConfig { Service = ServiceDefinitions.Github,    URL = "https://service1.example.com" },
                }
            };

            var writer = new StreamWriter(new MemoryStream());
            fileSystemMock
                .Setup(x => x.File.CreateText(filepath))
                .Returns(writer);

            // Act
            configRepository.Save(config);

            // Assert
            fileSystemMock.Verify(x => x.File.CreateText("config.yaml"), Times.Once);
        }
    }
}
