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
using WebSocketSharp;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Reflection.Metadata;
using System.IO;

namespace BocchiTracker.GameCaptureRTC.Protocol
{
    public class WebRTCWebSocketPeer : WebSocketBehavior
    {
        private RTCPeerConnection _pc = default!;

        public Func<Task<RTCPeerConnection>> CreatePeerConnection = default!;

        public RTCPeerConnection RTCPeerConnection => _pc;

        public RTCOfferOptions OfferOptions { get; set; } = default!;

        public RTCAnswerOptions AnswerOptions { get; set; } = default!;

        protected override async void OnMessage(MessageEventArgs e)
        {
            RTCSessionDescriptionInit init2;
            if (RTCIceCandidateInit.TryParse(e.Data, out var init))
            {
                Console.WriteLine("Got remote ICE candidate.");
                _pc.addIceCandidate(init);
            }
            else if (RTCSessionDescriptionInit.TryParse(e.Data, out init2))
            {
                Console.WriteLine($"Got remote SDP, type {init2.type}.");
                SetDescriptionResultEnum setDescriptionResultEnum = _pc.setRemoteDescription(init2);
                if (setDescriptionResultEnum != 0)
                {
                    Console.WriteLine($"Failed to set remote description, {setDescriptionResultEnum}.");
                    _pc.Close("failed to set remote description");
                    base.Close();
                }
                else if (_pc.signalingState == RTCSignalingState.have_remote_offer)
                {
                    RTCSessionDescriptionInit answerSdp = _pc.createAnswer(AnswerOptions);
                    await _pc.setLocalDescription(answerSdp).ConfigureAwait(continueOnCapturedContext: false);
                    Console.WriteLine($"Sending SDP answer to client {Context.UserEndPoint}.");
                    Context.WebSocket.Send(AddPlayerIdJson(answerSdp.toJSON()));
                }
            }
        }

        protected override async void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine($"Web socket client connection from {Context.UserEndPoint}.");
            _pc = await CreatePeerConnection().ConfigureAwait(continueOnCapturedContext: false);

            var rtc_config = _pc.getConfiguration();
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(
                new Dictionary<string, object>
                {
                    { "type",                   "config" },
                    { "peerConnectionOptions", new Dictionary<string, object> {
                            { "bundlePolicy",           rtc_config.bundlePolicy.ToString() },
                            { "certificates",           rtc_config.certificates },
                            { "iceCandidatePoolSize",   rtc_config.iceCandidatePoolSize },
                            { "iceServers",             rtc_config.iceServers },
                            { "iceTransportPolicy",     rtc_config.iceTransportPolicy.ToString() },
                            { "rtcpMuxPolicy",          rtc_config.rtcpMuxPolicy.ToString() },
                        }
                    },
                }
            );
            Context.WebSocket.Send(data);

            _pc.onicecandidate += delegate (RTCIceCandidate iceCandidate)
            {
                if (_pc.signalingState == RTCSignalingState.have_remote_offer || _pc.signalingState == RTCSignalingState.stable)
                {
                    Context.WebSocket.Send(AddPlayerIdJson(iceCandidate.toJSON()));
                }
            };
            
            RTCSessionDescriptionInit offerSdp = _pc.createOffer(OfferOptions);
            await _pc.setLocalDescription(offerSdp).ConfigureAwait(continueOnCapturedContext: false);
            Console.WriteLine($"Sending SDP offer to client {Context.UserEndPoint}.");
            Context.WebSocket.Send(AddPlayerIdJson(offerSdp.toJSON()));
        }

        private string AddPlayerIdJson(string inJson)
        {
            string custom_json = inJson;
            custom_json = custom_json.Insert(inJson.LastIndexOf('}'), ", \"playerid\": \"BocchiTrackerPlayer\"");
            return custom_json;
        }
    }

    public class WebRTC : WebSocketBehavior, ICaptureProtocol, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private WebSocketServer _web_socket = default!;
        private CaptureFrameStorage _captureFrameStorage = default!;
        private static bool _isConnecting;
        private SubscriptionToken _subscriptionToken;

        public WebRTC(IEventAggregator inEventAggregator)
        {
            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator.GetEvent<GameCaptureStopRequest>().Subscribe(Stop, ThreadOption.BackgroundThread);
        }

        public void Dispose()
        {
            _web_socket.Stop();
            _eventAggregator.GetEvent<GameCaptureStopRequest>().Unsubscribe(_subscriptionToken);
        }

        public bool IsConnect()
        {
            return _isConnecting;
        }

        public void Start(int inPort, string inFFmpegPath, Config.Parts.CaptureSetting inCaptureSetting)
        {
            string? ffmpeg = inFFmpegPath;
            if (ffmpeg == null || !Path.Exists(ffmpeg))
                throw new Exception("FFmpeg path is not set.");

            int maxFrameCount = (60 * inCaptureSetting.RecordingFrameRate) * inCaptureSetting.RecordingMintes;
            _captureFrameStorage = new CaptureFrameStorage(ffmpeg, maxFrameCount, maxFrameCount / 10);
            _web_socket = new WebSocketServer(IPAddress.Any, inPort, false);
            _web_socket.Log.Level = WebSocketSharp.LogLevel.Trace;
            _web_socket.AllowForwardedRequest = true;
            _web_socket.AddWebSocketService<WebRTCWebSocketPeer>("/", (peer) => peer.CreatePeerConnection = () => CreatePeerConnection(ffmpeg, inCaptureSetting, _captureFrameStorage));
            _web_socket.Start();
        }

        public void Stop()
        {
            string movie = _captureFrameStorage.ConcatMovie();
            if(movie.IsNullOrEmpty()) 
                return;
            _eventAggregator.GetEvent<GameCaptureFinishEvent>().Publish(movie);
        }

        private static Task<RTCPeerConnection> CreatePeerConnection(string inFFmpegPath, Config.Parts.CaptureSetting inCaptureSetting, CaptureFrameStorage inFrameStorage)
        {
            FFmpegInit.Initialise(FfmpegLogLevelEnum.AV_LOG_VERBOSE, inFFmpegPath);
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

            if (inCaptureSetting.IncludeAudio)
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
