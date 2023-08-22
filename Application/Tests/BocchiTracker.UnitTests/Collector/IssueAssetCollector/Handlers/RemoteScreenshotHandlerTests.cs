using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using BocchiTracker.IssueAssetCollector;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ModelEvent;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.IO;
using System.Threading;
using Prism.Events;
using BocchiTracker.ProcessLinkQuery.Queries;

namespace BocchiTracker.Tests.Collector.IssueAssetCollector.Handlers
{
    public class RemoteScreenshotHandlerTests
    {
        [Fact]
        public void Test_Handle_RequestQueryEventBus()
        {
            // Arrange
            var mockFilenameGenerator = new Mock<IFilenameGenerator>();

            mockFilenameGenerator.Setup(f => f.Generate()).Returns("test");
            
            var mockedEvent1 = new Mock<RequestQueryEvent>();
            var mockedEvent2 = new Mock<ReceiveScreenshotEvent>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock
              .Setup(x => x.GetEvent<RequestQueryEvent>())
              .Returns(mockedEvent1.Object);
            eventAggregatorMock
              .Setup(x => x.GetEvent<ReceiveScreenshotEvent>())
              .Returns(mockedEvent2.Object);

            var handler = new RemoteScreenshotHandler(eventAggregatorMock.Object, mockFilenameGenerator.Object);

            // Act
            handler.Handle(1, 1, "output");

            // Assert
            mockedEvent1.Verify(x => x.Publish(It.Is<RequestQueryEventParameter>(m => m.ClientID == 1 && m.QueryID == QueryID.ScreenshotData)));
        }

        [Fact]
        public void Test_Handle_RemoteScreenshot()
        {
            // Arrange
            var mockEvent = new Mock<IEventAggregator>();
            mockEvent
                .Setup(ea => ea.GetEvent<ReceiveScreenshotEvent>())
                .Returns(new ReceiveScreenshotEvent());
            var dummyScreenshotData = new ReceiveScreenshotEventParameter(
                4, //!< Width
                3, //!< Height
                new byte[4 * 3 * 4] {
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
                });
            Directory.CreateDirectory("output");

            // Instantiate the handler with the mocked dependencies
            var handler = new RemoteScreenshotSaveProcess(mockEvent.Object);
            handler.Output = "output\\test.png";

            // Act
            handler.Handle(dummyScreenshotData);

            // Assert
            Assert.True(File.Exists("output\\test.png")); // Check if the file is written to the output path
            using var image = Image.Load<Rgba32>("output\\test.png");
            Assert.Equal(dummyScreenshotData.Width, image.Width);
            Assert.Equal(dummyScreenshotData.Height, image.Height);

            // Cleanup
            File.Delete("output\\test.png");
            Directory.Delete("output");
        }
    }
}
