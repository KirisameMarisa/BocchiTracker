using BocchiTracker.ApplicationInfoCollector;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using BocchiTracker.IssueAssetCollector.Utils;
using System.Diagnostics;
using BocchiTracker.IssueAssetCollector.Utils.Win32;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class LocalScreenshotHandler : ScreenshotHandler
    {
        IClientCapture _capture;
        IGetWindowHandleFromPid _getWindowHandleFromPid;

        public LocalScreenshotHandler(IClientCapture inCapture, IGetWindowHandleFromPid inGetWindowHandleFromPid, IFilenameGenerator inFilenameGenerator)
            : base(inFilenameGenerator)
        {
            this._capture = inCapture;
            this._getWindowHandleFromPid = inGetWindowHandleFromPid;
        }

        public override void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {
            var handle = _getWindowHandleFromPid.Get(inPID);
            if (handle == IntPtr.Zero) 
                return;

            var data = _capture.CaptureWindow(handle);
            using var image = Image.LoadPixelData<Byte4>(data.ImageData, data.Width, data.Height);
            image.SaveAsPng(Path.Combine(inOutput, _filenameGenerator.Generate(inAppStatusBundle) + ".png"));
        }
    }
}
