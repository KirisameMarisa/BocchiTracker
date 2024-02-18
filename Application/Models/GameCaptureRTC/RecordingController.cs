using BocchiTracker.GameCaptureRTC.Protocol;
using BocchiTracker.ModelEvent;
using Prism.Events;

namespace BocchiTracker.GameCaptureRTC
{
    public class RecordingController : ICaptureProtocol
    {
        private readonly IEventAggregator _eventAggregator;

        private ICaptureProtocol? _captureProtocol;

        public RecordingController(IEventAggregator inEventAggregator)
        {
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload);
        }

        public bool IsConnect()
        {
            if (_captureProtocol == null)
                return false;
            return _captureProtocol.IsConnect();
        }

        public void Start()
        {
            _captureProtocol?.Start();
        }

        public void Stop()
        {
            _captureProtocol?.Stop();
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if (inParam.UserConfig == null || inParam.ProjectConfig == null)
                return;

            switch (inParam.UserConfig.UserCaptureSetting.GameCaptureType)
            {
                case Config.GameCaptureType.OBSStudio:
                    {
                        _captureProtocol = new Protocol.OBSCapture(inParam.ProjectConfig.Port, "");
                    }
                    break;
                case Config.GameCaptureType.WebRTC:
                    {
                        _captureProtocol = new Protocol.WebRTC(_eventAggregator, inParam.ProjectConfig.Port, false, inParam.ProjectConfig.CaptureSetting, inParam.UserConfig.UserCaptureSetting);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
