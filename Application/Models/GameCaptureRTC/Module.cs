using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.GameCaptureRTC
{
    public class Module : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) {}

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(CaptureFrameStorage));
            containerRegistry.RegisterSingleton(typeof(RecordingController));
        }
    }
}