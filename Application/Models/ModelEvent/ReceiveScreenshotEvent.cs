using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class ReceiveScreenshotEventParameter
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[]? ImageData { get; set; }

        public ReceiveScreenshotEventParameter(int inWidth, int inHeight, byte[]? inImageData)
        {
            Width = inWidth;
            Height = inHeight;
            ImageData = inImageData;
        }
    }

    public class ReceiveScreenshotEvent : PubSubEvent<ReceiveScreenshotEventParameter> {}
}
