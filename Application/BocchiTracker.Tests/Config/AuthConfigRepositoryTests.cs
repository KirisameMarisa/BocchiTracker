using BocchiTracker.ServiceClientAdapters;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Config
{
    public class AuthConfigRepositoryTests
    {
        [Fact]
        public void Load_ValidFile_ReturnsAuthConfigBase()
        {
            // Arrange
            var filePath = "authConfig.yaml";
            var fileContent = @"Username: testuser
Password: testpass
APIKey: testapikey";
            var fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { filePath, new MockFileData(fileContent) }
            });

            var repo = new ConfigRepository<AuthConfig>(filePath, fileSystemMock);

            // Act
            var result = repo.Load();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result?.Username);
            Assert.Equal("testpass", result?.Password);
            Assert.Equal("testapikey", result?.APIKey);

        }
    }
}
