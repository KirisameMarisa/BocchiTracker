using BocchiTracker.ApplicationInfoCollector;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using BocchiTracker.IssueAssetCollector.Utils;
using System.Diagnostics;

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

        public override void Handle(int inClientID, int inPID, string inOutput)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                var process = Process.GetProcessById(inPID);
                if (process == null)
                    return;
                handle = process.MainWindowHandle;
            }
            catch { return; }

            if(handle == IntPtr.Zero) 
                return;

            var data = _capture.CaptureWindow(handle);
            using var image = Image.LoadPixelData<Byte4>(data.ImageData, data.Width, data.Height);
            image.SaveAsPng(Path.Combine(inOutput, _filenameGenerator.Generate() + ".png"));
        }
    }
}
