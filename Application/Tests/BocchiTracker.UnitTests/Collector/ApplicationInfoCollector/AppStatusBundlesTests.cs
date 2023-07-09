using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ApplicationInfoCollector;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector
{
    public class AppStatusBundlesTests
    {
        [Fact]
        public void Test_Add()
        {
            // Arrange
            var appStatusBundles = new AppStatusBundles();

            // Act
            appStatusBundles.Add(1);

            // Assert
            Assert.True(appStatusBundles.Bundles.ContainsKey(1));
        }

        [Fact]
        public void Test_Remove()
        {
            // Arrange
            var appStatusBundles = new AppStatusBundles();
            appStatusBundles.Add(1);

            // Act
            appStatusBundles.Remove(1);

            // Assert
            Assert.False(appStatusBundles.Bundles.ContainsKey(1));
        }

        [Fact]
        public void Test_GetBundlesByClientID()
        {
            // Arrange
            var appStatusBundles = new AppStatusBundles();
            appStatusBundles.Add(1);

            // Act
            var bundle = appStatusBundles.GetBundlesByClientID(1);

            // Assert
            Assert.NotNull(bundle);
            Assert.Equal(1, bundle?.AppBasicInfo.ClientID);
        }

        [Fact]
        public void Test_GetBundlesByAppName()
        {
            // Arrange
            var appStatusBundles = new AppStatusBundles();
            appStatusBundles.Add(1);
            appStatusBundles[1].AppBasicInfo.AppName = "TestApp";

            // Act
            var bundles = appStatusBundles.GetBundlesByAppName("TestApp");

            // Assert
            Assert.Single(bundles);
            Assert.Equal("TestApp", bundles.First().AppBasicInfo.AppName);
        }
    }
}
