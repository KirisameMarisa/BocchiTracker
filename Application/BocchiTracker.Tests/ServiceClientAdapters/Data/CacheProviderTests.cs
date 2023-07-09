using BocchiTracker.ServiceClientAdapters.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace BocchiTracker.Tests.ServiceClientAdapters.Data
{
    public class CacheProviderTests
    {
        [Fact]
        public void IsExpired_FileDoesNotExist_ReturnsTrue()
        {
            var file_system = new Mock<IFileSystem>();
            var cache_provider = new CacheProvider("", file_system.Object);

            // Arrange
            string label = "testLabel";
            string file_path = Path.Combine("BocchiTracker", string.Format("{0}.Cache.yaml", label));

            file_system
                .Setup(x => x.File.Exists(file_path))
                .Returns(false);

            // Act
            var result = cache_provider.IsExpired(label);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsExpired_FileExistsAndNotExpired_ReturnsFalse()
        {
            var file_system = new Mock<IFileSystem>();
            var cache_provider = new CacheProvider("",file_system.Object);

            // Arrange
            string label = "testLabel";
            string file_path = Path.Combine("BocchiTracker", string.Format("{0}.Cache.yaml", label));
            DateTime lastModified = DateTime.Now.AddDays(-10);

            file_system
                .Setup(x => x.File.Exists(file_path))
                .Returns(true);
            file_system
                .Setup(x => x.File.GetLastWriteTime(file_path))
                .Returns(lastModified);

            // Act
            var result = cache_provider.IsExpired(label);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Set_ValidData_CreatesCacheFile()
        {
            var file_system = new Mock<IFileSystem>();
            var cache_provider = new CacheProvider("", file_system.Object);

            // Arrange
            string label = "testLabel";
            string file_path = Path.Combine("BocchiTracker", string.Format("{0}.Cache.yaml", label));
            var data = new TestData { Value = "Test" };

            var writer = new StreamWriter(new MemoryStream());
            file_system
                .Setup(x => x.File.CreateText(file_path))
                .Returns(writer);
            file_system
                .Setup(x => x.Directory.CreateDirectory(Path.GetDirectoryName(file_path)));

            // Act
            cache_provider.Set(label, data);

            // Assert
            file_system.Verify(x => x.File.CreateText(file_path), Times.Once);
        }

        [Fact]
        public void Get_ValidData_ReturnsCachedValue()
        {
            var file_system     = new Mock<IFileSystem>();
            var cache_provider  = new CacheProvider("", file_system.Object);

            // Arrange
            string label = "testLabel";
            string filePath = Path.Combine("BocchiTracker", string.Format("{0}.Cache.yaml", label));
            var data = new TestData { Value = "Test" };

            using var memory_stream = new MemoryStream();
            using var writer = new StreamWriter(memory_stream);
            writer.WriteLine("Value: Test");
            writer.Flush();
            memory_stream.Position = 0;

            file_system
                .Setup(x => x.File.Exists(filePath))
                .Returns(true);
            file_system
                .Setup(x => x.File.OpenText(filePath))
                .Returns(new StreamReader(memory_stream));

            // Act
            var result = cache_provider.Get<TestData>(label);

            // Assert
            file_system.Verify(x => x.File.Exists(filePath), Times.Once);
            file_system.Verify(x => x.File.OpenText(filePath), Times.Once);
            Assert.Equal(data.Value, result.Value);
        }

        // Add more test cases for other methods

        public class TestData
        {
            public string? Value { get; set; }
        }
    }
}
