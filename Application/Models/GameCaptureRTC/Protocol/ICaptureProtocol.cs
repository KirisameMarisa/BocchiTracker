namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public interface ICaptureProtocol
    {
        void Start(int inPort, string inFFmpegPath, Config.Parts.CaptureSetting inCaptureSetting);

        void Stop();

        bool IsConnect();
    }
}
