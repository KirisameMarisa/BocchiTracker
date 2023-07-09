using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.Collector.ApplicationInfoCollector
{
    public class TrackerApplicationTests
    {
        private readonly TrackerApplication _trackerApplication;

        public TrackerApplicationTests()
        {
            _trackerApplication = new TrackerApplication();
        }

        [Fact]
        public void SetTracker_ValidClientID_ShouldSetTracker()
        {
            // Arrange
            int testClientID = 1;
            _trackerApplication.Bundles.Add(testClientID);
            var testBundle = _trackerApplication.Bundles.GetBundlesByClientID(testClientID);

            // Act
            _trackerApplication.SetTracker(testClientID);

            // Assert
            Assert.Equal(testBundle, _trackerApplication.Tracker);
        }

        [Fact]
        public void SetTracker_InvalidClientID_ShouldNotSetTracker()
        {
            // Arrange
            int testClientID = 1;
            
            // Act
            _trackerApplication.SetTracker(testClientID);

            // Assert
            Assert.Null(_trackerApplication.Tracker);
        }
    }
}
