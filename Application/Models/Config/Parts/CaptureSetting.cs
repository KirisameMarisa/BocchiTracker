using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Config.Parts
{
    public class CaptureSetting
    {
        public GameCaptureType GameCaptureType { get; set; } = GameCaptureType.NotUse;

        public SIPSorceryMedia.Abstractions.VideoCodecsEnum VideoCodecs { get; set; } = SIPSorceryMedia.Abstractions.VideoCodecsEnum.VP8;

        public bool IncludeAudio = false;

        public int RecordingFrameRate { get; set; } = 30;

        public int RecordingMintes { get; set; } = 3;
    }
}
