using BocchiTracker.Config.Configs;
using BocchiTracker.GameCaptureRTC.Protocol;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System.Threading.Tasks;

namespace BocchiTracker.GameCaptureRTC
{
    public class RecordingController : ICaptureProtocol
    {
        private readonly IEventAggregator _eventAggregator;

        private ICaptureProtocol? _captureProtocol;

        public RecordingController(IEventAggregator inEventAggregator)
        {
            _eventAggregator = inEventAggregator;
        }

        public bool IsConnect()
        {
            if (_captureProtocol == null)
                return false;
            return _captureProtocol.IsConnect();
        }

        public void Start(int inPort, ProjectConfig inProjectConfig, UserConfig inUserConfig)
        {
            switch (inUserConfig.UserCaptureSetting.GameCaptureType)
            {
                case Config.GameCaptureType.OBSStudio:
                    {
                        _captureProtocol = new Protocol.OBSCapture();
                    }
                    break;
                case Config.GameCaptureType.WebRTC:
                    {
                        _captureProtocol = new Protocol.WebRTC(_eventAggregator);
                    }
                    break;
                default:
                    break;
            }

            if(_captureProtocol != null)
                _captureProtocol.Start(inPort, inProjectConfig, inUserConfig);
        }

        public void Stop()
        {
            _captureProtocol?.Stop();
        }
    }
}
