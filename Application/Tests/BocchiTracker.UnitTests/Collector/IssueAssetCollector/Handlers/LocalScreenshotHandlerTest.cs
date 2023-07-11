using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.IssueAssetCollector.Utils;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ModelEvent;
using System.IO;

namespace BocchiTracker.Tests.Collector.IssueAssetCollector.Handlers
{
    public class LocalScreenshotHandlerTest
    {
        [Fact]
        public void Test_Handle()
        {
            // Arrange
            var mockCapture = new Mock<IClientCapture>();
            var mockFilenameGenerator = new Mock<IFilenameGenerator>();

            var testData = new CaptureData
            {
                Width = 4,
                Height = 3,
                ImageData = new byte[4 * 3 * 4] 
                {
                      0,   0,   0,   0,
                      0,   0,   0,   0,
                    255, 255, 255, 255,
                    255, 255,   0, 255,
                      0,   0,   0,   0,
                    255,   0, 255,   0,
                    255, 255,   0, 255,
                      0, 255,   0, 255,
                    255,   0, 255,   0,
                    255,   0,   0,   0,
                      0, 255,   0, 255,
                    255, 255, 255,  255,
                }               
            };
            var expectedFilePath = Path.Combine("output2", "test.png");
            Directory.CreateDirectory("output2");

            mockCapture.Setup(c => c.CaptureWindow(It.IsAny<IntPtr>())).Returns(testData);
            mockFilenameGenerator.Setup(f => f.Generate()).Returns("test");

            var handler = new LocalScreenshotHandler(mockCapture.Object, mockFilenameGenerator.Object);

            // Act
            handler.Handle(0, 0, IntPtr.Zero, "output2");

            // Assert
            Assert.True(File.Exists(expectedFilePath));

            // Clean up
            File.Delete(expectedFilePath);
            Directory.Delete("output2");
        }
    }
}
