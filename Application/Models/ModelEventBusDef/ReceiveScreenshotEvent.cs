using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class ScreenshotData
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[]? ImageData { get; set; }
    }

    public class ReceiveScreenshotEvent : IRequest
    {
        public ScreenshotData ScreenshotData { get; set; }

        public ReceiveScreenshotEvent(ScreenshotData inData)
        {
            ScreenshotData = inData;
        }
    }
}
