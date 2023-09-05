using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ApplicationInfoCollector;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector
{
    namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector
    {
        public class AppStatusBundlesTests
        {
            [Fact]
            public void Add_WhenBundleAdded_ShouldContainBundleWithGivenID()
            {
                // Arrange
                var appStatusBundles = new AppStatusBundles();

                // Act
                appStatusBundles.Add(1, new Dictionary<string, string>());

                // Assert
                Assert.True(appStatusBundles.Bundles.ContainsKey(1));
            }

            [Fact]
            public void Remove_WhenBundleRemoved_ShouldNotContainBundleWithGivenID()
            {
                // Arrange
                var appStatusBundles = new AppStatusBundles();
                appStatusBundles.Add(1, new Dictionary<string, string>());

                // Act
                appStatusBundles.Remove(1);

                // Assert
                Assert.False(appStatusBundles.Bundles.ContainsKey(1));
            }

            [Fact]
            public void GetBundlesByClientID_WhenBundleExists_ShouldReturnBundleWithGivenClientID()
            {
                // Arrange
                var appStatusBundles = new AppStatusBundles();
                appStatusBundles.Add(1, new Dictionary<string, string>());

                // Act
                var bundle = appStatusBundles.GetBundlesByClientID(1);

                // Assert
                Assert.NotNull(bundle);
                Assert.Equal(1, bundle?.AppBasicInfo.ClientID);
            }
        }
    }
}
