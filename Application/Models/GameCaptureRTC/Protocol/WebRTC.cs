using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Prism.Events;
using BocchiTracker.ModelEvent;
using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;
using SIPSorceryMedia.FFmpeg;
using BocchiTracker.Config.Configs;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public class WebRTC : WebSocketBehavior, ICaptureProtocol, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private WebSocketServer _web_socket;
        private CaptureFrameStorage _captureFrameStorage;
        private static bool _isConnecting;

        public WebRTC(IEventAggregator inEventAggregator, int inPort, bool inSecure, CaptureSetting inCaptureSetting, UserCaptureSetting inUserCaptureSetting)
        {
            _eventAggregator = inEventAggregator;
            _captureFrameStorage = new CaptureFrameStorage(inUserCaptureSetting.RecordingFrameRate, inUserCaptureSetting.RecordingMintes);
            _web_socket = new WebSocketServer(IPAddress.Any, inPort, inSecure);
            _web_socket.AddWebSocketService<WebRTCWebSocketPeer>("/", (peer) => peer.CreatePeerConnection = () => CreatePeerConnection(inCaptureSetting, inUserCaptureSetting, _captureFrameStorage));
            _web_socket.Start();
        }

        public void Dispose()
        {
            _web_socket.Stop();
        }

        public bool IsConnect()
        {
            return _isConnecting;
        }

        public void Start() 
        {

        }

        public void Stop()
        {
            _eventAggregator
                .GetEvent<GameCaptureFinishEvent>()
                .Publish(new GameCaptureFinishEventParameter { CaptureStreamParameter = _captureFrameStorage.CaptureStreamParameter });
            _captureFrameStorage.CaptureStreamParameter.Frames.Clear();
        }

        private static Task<RTCPeerConnection> CreatePeerConnection(CaptureSetting inCaptureSetting, UserCaptureSetting inUserCaptureSetting, CaptureFrameStorage inFrameStorage)
        {
            FFmpegInit.Initialise(FfmpegLogLevelEnum.AV_LOG_VERBOSE, inCaptureSetting.FFmpegPath);
            var videoEP = new FFmpegVideoEndPoint();
            videoEP.RestrictFormats(format => format.Codec == inCaptureSetting.VideoCodecs);

            videoEP.OnVideoSinkDecodedSampleFaster += (RawImage rawImage) =>
            {
                if (rawImage.PixelFormat == SIPSorceryMedia.Abstractions.VideoPixelFormatsEnum.Rgb)
                {
                    inFrameStorage.AddFrame((int)rawImage.Width, (int)rawImage.Height, (int)rawImage.Stride, rawImage.Sample);
                }
            };

            RTCConfiguration config = new RTCConfiguration
            {
                //iceServers = new List<RTCIceServer> { new RTCIceServer { urls = STUN_URL } }
                X_UseRtpFeedbackProfile = true
            };
            var pc = new RTCPeerConnection(config);

            if (inUserCaptureSetting.IncludeAudio)
            {
                MediaStreamTrack audioTrack = new MediaStreamTrack(SDPMediaTypesEnum.audio, false,
                    new List<SDPAudioVideoMediaFormat> { new SDPAudioVideoMediaFormat(SDPWellKnownMediaFormatsEnum.PCMU) }, MediaStreamStatusEnum.RecvOnly);
                pc.addTrack(audioTrack);
            }
            MediaStreamTrack videoTrack = new MediaStreamTrack(videoEP.GetVideoSinkFormats(), MediaStreamStatusEnum.RecvOnly);
            pc.addTrack(videoTrack);

            pc.OnVideoFrameReceived += videoEP.GotVideoFrame;
            pc.OnVideoFormatsNegotiated += (formats) => videoEP.SetVideoSinkFormat(formats.First());
            pc.onconnectionstatechange += async (state) =>
            {
                Console.WriteLine($"Peer connection state change to {state}.");
                switch (state)
                {
                    case RTCPeerConnectionState.failed: pc.Close("ice disconnection"); break;
                    case RTCPeerConnectionState.closed: await videoEP.CloseVideo(); break;
                    default: break;
                }
            };
            pc.OnSendReport += (media, sr) => Console.WriteLine($"RTCP Send for {media}\n{sr.GetDebugSummary()}");
            pc.oniceconnectionstatechange += (state) =>
            {
                Console.WriteLine($"ICE connection state change to {state}.");
                switch (state)
                {
                    case RTCIceConnectionState.connected: _isConnecting = true; break;
                    default: _isConnecting = false; break;
                }
            };
            return Task.FromResult(pc);
        }
    }
}
