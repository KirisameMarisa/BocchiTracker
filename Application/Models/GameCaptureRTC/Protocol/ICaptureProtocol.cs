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
        void Start(int inPort, ProjectConfig inProjectConfig, UserConfig inUserConfig);

        void Stop();

        bool IsConnect();
    }
}
