using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBSWebsocketDotNet;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public class OBSCapture : ICaptureProtocol
    {
        private OBSWebsocket _obsWebSocket;

        public OBSCapture(int inPort, string inCaptureSource) 
        {
            _obsWebSocket = new OBSWebsocket();
            var scenes = _obsWebSocket.ListScenes();
        }

        public bool IsConnect()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
