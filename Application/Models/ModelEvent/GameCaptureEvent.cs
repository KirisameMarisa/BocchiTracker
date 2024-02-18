using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class OBSStudioParameter
    {
        public string MoviePath { get; set; } = string.Empty;
    }

    public class CaptureStreamParameter
    {
        public class Frame
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int Stride { get; set; }
            public byte[]? Data { get; set; }
        }
        public List<Frame> Frames { get; set; } = new List<Frame>();
    }

    public class GameCaptureFinishEventParameter
    {
        public CaptureStreamParameter? CaptureStreamParameter { get; set; }
        public OBSStudioParameter? OBSStudioParameter { get; set; }
    }

    public class GameCaptureStartEvent : PubSubEvent { }

    public class GameCaptureFinishEvent : PubSubEvent<GameCaptureFinishEventParameter> { }
}
