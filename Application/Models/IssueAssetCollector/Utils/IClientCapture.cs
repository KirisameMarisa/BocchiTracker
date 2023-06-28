using System;

namespace BocchiTracker.IssueAssetCollector.Utils
{
    public class CaptureData
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[]? ImageData { get; set; }
    }

    public interface IClientCapture
    {
        CaptureData CaptureWindow(IntPtr hwnd);
    }
}
