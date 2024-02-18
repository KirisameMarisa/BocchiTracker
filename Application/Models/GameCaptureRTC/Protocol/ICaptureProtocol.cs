using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public interface ICaptureProtocol
    {
        void Start();

        void Stop();

        bool IsConnect();
    }
}
