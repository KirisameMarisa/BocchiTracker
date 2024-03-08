using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.Config.Configs;
using OBSWebsocketDotNet;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public class OBSCapture : ICaptureProtocol
    {
        private OBSWebsocket _obsWebSocket = default!;

        public OBSCapture() {}

        public bool IsConnect()
        {
            throw new NotImplementedException();
        }
  
        public void Start(int inPort, string inFFmpegPath, Config.Parts.CaptureSetting inCaptureSetting)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
