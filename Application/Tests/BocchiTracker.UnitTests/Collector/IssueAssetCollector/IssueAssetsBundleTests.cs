using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BocchiTracker.IssueAssetCollector;
using Xunit;

namespace BocchiTracker.Tests.Collector.IssueAssetCollector
{
    public class IssueAssetsBundleTests
    {
        private AssetData TestFile1 = new AssetData(Path.Combine(Path.GetTempPath(), "TestFile1.txt"));
        private AssetData TestFile2 = new AssetData(Path.Combine(Path.GetTempPath(), "TestFile2.txt"));

        // Make sure to clean up the files after testing.
        public IssueAssetsBundleTests()
        {
            if (File.Exists(TestFile1.FullName))
            {
                File.Delete(TestFile1.FullName);
            }

            if (File.Exists(TestFile2.FullName))
            {
                File.Delete(TestFile2.FullName);
            }
        }

        [Fact]
        public void Add_WhenFileExists_ShouldAddToBundle()
        {
            // Arrange
            File.WriteAllText(TestFile1.FullName, "Test content");
            var bundle = new IssueAssetsBundle();

            // Act
            bundle.Add(TestFile1.FullName);

            // Assert
            Assert.Contains(TestFile1.Name, bundle.Bundle[0].Name);
            Assert.Contains(TestFile1.FullName, bundle.Bundle[0].FullName);
        }

        [Fact]
        public void Add_WhenFileDoesNotExist_ShouldNotAddToBundle()
        {
            // Arrange
            var bundle = new IssueAssetsBundle();

            // Act
            bundle.Add(TestFile1.FullName);

            // Assert
            Assert.DoesNotContain(TestFile1, bundle.Bundle);
        }

        [Fact]
        public void Delete_WhenFileInBundle_ShouldRemoveFromBundle()
        {
            // Arrange
            File.WriteAllText(TestFile1.FullName, "Test content");
            var bundle = new IssueAssetsBundle();
            bundle.Add(TestFile1.FullName);

            // Act
            bundle.Delete(TestFile1.FullName);

            // Assert
            Assert.DoesNotContain(TestFile1, bundle.Bundle);
        }
    }
}
