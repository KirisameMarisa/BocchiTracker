using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using WebSocketSharp;
using Unity.Plastic.Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using BocchiTracker;
using UnityEngine.Windows.WebCam;

public class BocchiTrackerVideoCapture : MonoBehaviour
{
    /// <summary>
    /// Address of the WebSocket server
    /// </summary>
    private string serverAddr { get; set; }

    /// <summary>
    /// Camera to capture
    /// </summary>
    private Camera captureCamera { get; set; }

    /// <summary>
    /// Represents the state of connection to the server.
    /// </summary>
    [System.Serializable]
    public class RTCMessage
    {
        public string type;
        public string candidate;
        public string sdp;
        public string sdpMid;
        public int sdpMLineIndex;
        public string playerid;
    }

    public enum ConnectionState
    {
        ConnectStart,
        Connecting,
        Connected,
        Close,
    }
    private ConnectionState connectionState = ConnectionState.ConnectStart;

    /// <summary>
    /// Time until connection timeout
    /// </summary>
    private float connectingTimeout = 0f;

    /// <summary>
    /// WebSocket connection
    /// </summary>
    private WebSocket webSocket;

    /// <summary>
    /// Camera texture
    /// </summary>
    private RenderTexture cameraTexture;

    /// <summary>
    ///  WebRTC peer connection
    /// </summary>
    private RTCPeerConnection rtcPeerConnection;

    /// <summary>
    /// Message queue
    /// </summary>
    private Queue<string> messageQueue = new Queue<string>();

    /// <summary>
    /// Flag indicating whether messages are being processed
    /// </summary>
    private bool isProcessing = false;

    private void Start()
    {
        var setting = GetComponent<BocchiTrackerSetting>();
        serverAddr = $"ws://{setting.ServerAddress}:{setting.WebSocketPort}";
        captureCamera = setting.CaptureCamera;

        CreatePeerConnection();
    }

    private void Update()
    {
        StartCoroutine(ConnectServerTask());

        if (!isProcessing && messageQueue.Count > 0)
        {
            isProcessing = true;
            StartCoroutine(ProcessMessages());
        }
    }

    private void LateUpdate()
    {
        StartCoroutine(OnCapture());
    }

    void OnDestroy()
    {
        if (rtcPeerConnection != null)
        {
            foreach (var transceiver in rtcPeerConnection.GetTransceivers())
            {
                rtcPeerConnection.RemoveTrack(transceiver.Sender);
            }

            // Close datachannel befor offer
            rtcPeerConnection.Close();
            rtcPeerConnection = null;
        }

        if (webSocket != null)
        {
            webSocket.Close();
            webSocket = null;
        }
    }

    private IEnumerator ProcessMessages()
    {
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            yield return MessageProcesser(message);
        }
        isProcessing = false;
    }

    private void CreatePeerConnection()
    {
        RTCConfiguration config = default;
        rtcPeerConnection = new RTCPeerConnection(ref config);
        rtcPeerConnection.OnIceConnectionChange = (state) => { Debug.Log($"OnIceConnectionChange {state}"); };
        rtcPeerConnection.OnIceGatheringStateChange = (state) => { Debug.Log($"OnIceGatheringStateChange {state}"); };
        rtcPeerConnection.OnConnectionStateChange = (state) => { Debug.Log($"OnConnectionStateChange {state}"); };
        rtcPeerConnection.OnIceCandidate = (ev) =>
        {
            if (ev.Candidate != null)
            {
                var data = JsonConvert.SerializeObject(new Dictionary<string, string>
                {
                    { "candidate", ev.Candidate }
                });
                webSocket.Send(data);
            }
        };

        cameraTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.BGRA32);
        var track = new VideoStreamTrack(cameraTexture);
        rtcPeerConnection.AddTrack(track);
        {
            var codecs = RTCRtpSender.GetCapabilities(TrackKind.Video).codecs;
            foreach (var transceiver in rtcPeerConnection.GetTransceivers())
            {
                transceiver.SetCodecPreferences(codecs);
            }
        }
        StartCoroutine(WebRTC.Update());
    }

    private IEnumerator ConnectServerTask()
    {
        yield return -1;
        switch (connectionState)
        {
            case ConnectionState.ConnectStart:
                {
                    Debug.Assert(webSocket == null);
                    webSocket = new WebSocket(serverAddr);
                    webSocket.WaitTime = TimeSpan.FromSeconds(10);
                    webSocket.OnError += (sender, e) => 
                    { 
                        Debug.LogError(@$"Error! {e.Message}"); 
                    };
                    webSocket.OnOpen += (sender, e) => 
                    { 
                        connectionState = ConnectionState.Connected; 
                        connectingTimeout = 0f; 
                        Debug.Log(@$"OnOpen: {connectionState}"); 
                    };
                    webSocket.OnClose += (sender, e) => 
                    { 
                        connectionState = ConnectionState.Close; 
                        Debug.Log(@$"OnClose: {connectionState}"); 
                    };
                    webSocket.OnMessage += (sender, e) =>
                    {
                        if (e.Data == null)
                            return;
                        messageQueue.Enqueue(e.Data);
                    };
                    connectionState = ConnectionState.Connecting;
                    webSocket.ConnectAsync();
                }
                break;

            case ConnectionState.Connecting: 
                {
                    connectingTimeout += Time.deltaTime;
                    if (connectingTimeout >= 30f)
                    {
                        connectingTimeout = 0f;
                        connectionState = ConnectionState.Close;
                    }
                } 
                break;

            case ConnectionState.Connected: 
                break;
            
            case ConnectionState.Close:
                {
                    if(webSocket != null)
                    {
                        webSocket.Close();
                        webSocket = null;
                    }
                    connectionState = ConnectionState.ConnectStart;
                }
                break;
        }
        yield break;
    }

    private IEnumerator MessageProcesser(string inMessage)
    {
        RTCMessage desc = JsonUtility.FromJson<RTCMessage>(inMessage);
        Debug.Log($"Processing Msg {desc.type}");
        switch (desc.type)
        {
            case "ice":
                {
                    RTCIceCandidate candidate = new RTCIceCandidate(
                        new RTCIceCandidateInit
                        {
                            candidate = desc.candidate,
                            sdpMid = desc.sdpMid,
                            sdpMLineIndex = desc.sdpMLineIndex
                        }
                    );
                    rtcPeerConnection.AddIceCandidate(candidate);
                }
                break;
            case "config":
                break;
            default:
                {
                    Debug.Log($"SetRemoteDescription...");
                    RTCSessionDescription session = new RTCSessionDescription { type = RTCSdpType.Offer, sdp = desc.sdp };
                    var op1 = rtcPeerConnection.SetRemoteDescription(ref session);
                    yield return new WaitUntil(() => op1.IsDone);

                    Debug.Log($"CreateAnswer...");
                    var op2 = rtcPeerConnection.CreateAnswer();
                    yield return new WaitUntil(() => op2.IsDone);

                    Debug.Log($"SetLocalDescription...");
                    var ans_session = op2.Desc;
                    var op3 = rtcPeerConnection.SetLocalDescription(ref ans_session);
                    yield return new WaitUntil(() => op3.IsDone);

                    Debug.Log($"Send Aswer BocchiTracker...");
                    var data = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"type", ans_session.type.ToString() },
                        {"sdp", ans_session.sdp},
                    });
                    webSocket.Send(data);
                }
                break;
        }
        yield break;
    }

    private IEnumerator OnCapture()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture cam_render_texture = captureCamera.targetTexture;
        captureCamera.targetTexture = cameraTexture;
        captureCamera.Render();
        captureCamera.targetTexture = cam_render_texture;

        yield break;
    }
}