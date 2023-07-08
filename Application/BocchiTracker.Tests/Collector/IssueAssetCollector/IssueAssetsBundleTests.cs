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
        private string TestFile1 = Path.Combine(Path.GetTempPath(), "TestFile1.txt");
        private string TestFile2 = Path.Combine(Path.GetTempPath(), "TestFile2.txt");

        // Make sure to clean up the files after testing.
        public IssueAssetsBundleTests()
        {
            if (File.Exists(TestFile1))
            {
                File.Delete(TestFile1);
            }

            if (File.Exists(TestFile2))
            {
                File.Delete(TestFile2);
            }
        }

        [Fact]
        public void Add_WhenFileExists_ShouldAddToBundle()
        {
            // Arrange
            File.WriteAllText(TestFile1, "Test content");
            var bundle = new IssueAssetsBundle();

            // Act
            bundle.Add(TestFile1);

            // Assert
            Assert.Contains(TestFile1, bundle.Bundle);
        }

        [Fact]
        public void Add_WhenFileDoesNotExist_ShouldNotAddToBundle()
        {
            // Arrange
            var bundle = new IssueAssetsBundle();

            // Act
            bundle.Add(TestFile1);

            // Assert
            Assert.DoesNotContain(TestFile1, bundle.Bundle);
        }

        [Fact]
        public void Delete_WhenFileInBundle_ShouldRemoveFromBundle()
        {
            // Arrange
            File.WriteAllText(TestFile1, "Test content");
            var bundle = new IssueAssetsBundle();
            bundle.Add(TestFile1);

            // Act
            bundle.Delete(TestFile1);

            // Assert
            Assert.DoesNotContain(TestFile1, bundle.Bundle);
        }
    }
}
