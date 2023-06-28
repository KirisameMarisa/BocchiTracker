using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using BocchiTracker.IssueAssetCollector;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ModelEventBus;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace BocchiTracker.Tests.Collector.IssueAssetCollector.Handlers
{
    public class RemoteScreenshotHandlerTests
    {
        [Fact]
        public void Test_Handle_RequestQueryEventBus()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var mockFilenameGenerator = new Mock<IFilenameGenerator>();

            mockFilenameGenerator.Setup(f => f.Generate()).Returns("test");

            var handler = new RemoteScreenshotHandler(mockMediator.Object, mockFilenameGenerator.Object);

            // Act
            handler.Handle(1, 1, IntPtr.Zero, "output");

            // Assert
            mockMediator.Verify(m => m.Send(It.IsAny<RequestQueryEvent>(), default), Times.Once);
        }

        [Fact]
        public async Task Test_Handle_RemoteScreenshot()
        {
            // Arrange
            var dummyScreenshotData = new ModelEventBus.ScreenshotData
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
            var dummyRequest = new ReceiveScreenshotEvent(dummyScreenshotData);
            var dummyCancellationToken = new CancellationToken();
            ReceiveScreenshotEventBusHandler.Output = "output\\test.png";
            Directory.CreateDirectory("output");

            // Instantiate the handler with the mocked dependencies
            var handler = new ReceiveScreenshotEventBusHandler();

            // Act
            await handler.Handle(dummyRequest, dummyCancellationToken);

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
