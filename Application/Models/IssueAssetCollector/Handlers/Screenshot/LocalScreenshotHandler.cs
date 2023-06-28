using BocchiTracker.ApplicationInfoCollector;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using MediatR;
using BocchiTracker.IssueAssetCollector.Utils;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class LocalScreenshotHandler : ScreenshotHandler
    {
        IClientCapture _capture;

        public LocalScreenshotHandler(IClientCapture inCapture, IFilenameGenerator inFilenameGenerator)
            : base(inFilenameGenerator)
        {
            this._capture = inCapture;
        }

        public override void Handle(int inClientID, int inPID, IntPtr inHandle, string inOutput)
        {
            var data = _capture.CaptureWindow(inHandle);

            using var image = Image.LoadPixelData<Byte4>(data.ImageData, data.Width, data.Height);
            image.SaveAsPng(Path.Combine(inOutput, _filename_generator.Generate() + ".png"));
        }
    }
}
