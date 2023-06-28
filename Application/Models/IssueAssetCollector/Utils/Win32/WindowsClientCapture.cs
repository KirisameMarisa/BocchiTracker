#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace BocchiTracker.IssueAssetCollector.Utils.Win32
{

    public class WindowsClientCapture : IClientCapture
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public CaptureData CaptureWindow(IntPtr hwnd)
        {
            ForceActiveWindow.Process(hwnd);

            IntPtr hdc = GetDC(hwnd);

            RECT rect;
            GetWindowRect(hwnd, out rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            byte[] pixelData = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    uint colorref = GetPixel(hdc, x, y);
                    int index = (x + y * width) * 4;

                    //!< R, G, B, A
                    pixelData[index + 0] = (byte)(colorref >> 16 & 0xff);
                    pixelData[index + 1] = (byte)(colorref >> 8 & 0xff);
                    pixelData[index + 2] = (byte)(colorref & 0xff);
                    pixelData[index + 3] = 255;
                }
            }

            ReleaseDC(hwnd, hdc);
            return new CaptureData { ImageData = pixelData, Width = width, Height = height };
        }
    }
}
#endif