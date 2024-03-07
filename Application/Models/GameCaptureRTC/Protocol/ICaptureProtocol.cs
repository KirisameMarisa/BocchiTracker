using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public interface ICaptureProtocol
    {
        void Start(int inPort, string inFFmpegPath, Config.Parts.CaptureSetting inCaptureSetting);

        void Stop();

        bool IsConnect();
    }
}
