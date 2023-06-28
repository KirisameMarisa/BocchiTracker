using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
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
    public class ConfigRepositoryTests
    {
        [Fact]
        public void Load_ValidConfig_ReturnsConfigObject()
        {
            // Arrange
            string filepath = "config.yaml";
            var fileSystemMock = new Mock<IFileSystem>();
            var configRepository = new ConfigRepository<ProjectConfig>(filepath, fileSystemMock.Object);
            var config = new ProjectConfig
            {
                ServiceURLs = new List<Dictionary<ServiceDefinitions, string>>
                {
                    new Dictionary<ServiceDefinitions, string> { { ServiceDefinitions.Github, "https://service1.example.com" } },
                    new Dictionary<ServiceDefinitions, string> { { ServiceDefinitions.Redmine, "https://service2.example.com" } },
                }
            };

            var yaml = @"ServiceURLs:
  - Github: https://service1.example.com
  - Redmine: https://service2.example.com";
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
            Assert.Equal(config.ServiceURLs.Count, result.ServiceURLs.Count);
            Assert.Equal(config.ServiceURLs[0][ServiceDefinitions.Github], result.ServiceURLs[0][ServiceDefinitions.Github]);
            Assert.Equal(config.ServiceURLs[1][ServiceDefinitions.Redmine], result.ServiceURLs[1][ServiceDefinitions.Redmine]);
        }

        [Fact]
        public void Load_InvalidConfig_ReturnsNull()
        {
            // Arrange
            string filepath = "config.yaml";
            var fileSystemMock = new Mock<IFileSystem>();
            var configRepository = new ConfigRepository<ProjectConfig>(filepath, fileSystemMock.Object);

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
            var configRepository = new ConfigRepository<ProjectConfig>(filepath, fileSystemMock.Object);
            var config = new ProjectConfig { ServiceURLs = new List<Dictionary<ServiceDefinitions, string>>() };
            config.ServiceURLs.Add(new Dictionary<ServiceDefinitions, string> { { ServiceDefinitions.Github, "https://service1.example.com" } });

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
