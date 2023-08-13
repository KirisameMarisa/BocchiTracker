#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace BocchiTracker.IssueAssetCollector.Utils.Win32
{

    public class WindowsClientCapture : IClientCapture
    {
        private delegate void CaptureCallback(IntPtr data, int width, int height);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint usage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        public CaptureData CaptureWindow(IntPtr hwnd)
        {
            ForceActiveWindow.Process(hwnd);

            RECT rect;
            GetWindowRect(hwnd, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            BITMAPINFO info = new BITMAPINFO();
            info.biSize = Marshal.SizeOf(info);
            info.biWidth = width;
            info.biHeight = -height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0; // BI_RGB
            info.biSizeImage = width * height * 4;

            byte[] pixelData = new byte[info.biSizeImage];

            IntPtr hscreen = GetDC(hwnd);
            IntPtr hdc = CreateCompatibleDC(hscreen);
            IntPtr pvBits;
            IntPtr hbmp = CreateDIBSection(hdc, ref info, 0, out pvBits, IntPtr.Zero, 0);

            if(hbmp != IntPtr.Zero)
            {
                SelectObject(hdc, hbmp);
                PrintWindow(hwnd, hdc, 0x2);
                Marshal.Copy(pvBits, pixelData, 0, pixelData.Length);
                DeleteObject(hbmp);
            }
            DeleteDC(hdc);
            ReleaseDC(hwnd, hscreen);

            return new CaptureData { ImageData = pixelData, Width = width, Height = height };
        }
    }
}
#endif