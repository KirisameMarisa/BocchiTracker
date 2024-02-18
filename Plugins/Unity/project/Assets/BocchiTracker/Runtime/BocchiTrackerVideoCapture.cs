using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using WebSocketSharp;
using Unity.Plastic.Newtonsoft.Json;

public class BocchiTrackerVideoCapture : MonoBehaviour
{
    [System.Serializable]
    public class RTCMessage
    {
        public string type;
        public string candidate;
        public string sdp;
        public string sdpMid;
        public int sdpMLineIndex;
    }

    private WebSocket webSocket;

    private RenderTexture cameraTexture;

    private RTCPeerConnection rtcPeerConnection;

    private Queue<string> messageQueue = new Queue<string>();

    private bool isProcessing = false;

    public string ServerAddr { get; set; }

    public Camera CaptureCamera { get; set; }

    private void Start()
    {
        CreatePeerConnection();
        StartCoroutine(ConnectServerTask());
    }

    private void Update()
    {
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
        Debug.LogWarning("create new peerConnection start");

        RTCConfiguration config = default;
        rtcPeerConnection = new RTCPeerConnection(ref config);
        rtcPeerConnection.OnIceConnectionChange = (state) => { Debug.LogWarning($"OnIceConnectionChange {state}"); };
        rtcPeerConnection.OnIceGatheringStateChange = (state) => { Debug.LogWarning($"OnIceGatheringStateChange {state}"); };
        rtcPeerConnection.OnConnectionStateChange = (state) => { Debug.LogWarning($"OnConnectionStateChange {state}"); };
        rtcPeerConnection.OnIceCandidate = (ev) =>
        {
            Debug.LogWarning(ev.Candidate);
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

        Debug.LogWarning("create call back start");
        Debug.LogWarning("connect to signaling server start");

        webSocket = new WebSocket(ServerAddr);

        webSocket.OnMessage += (sender, e) =>
        {
            if (e.Data == null)
            {
                Debug.LogWarning("OnMessage Data null");
                return;
            }
            messageQueue.Enqueue(e.Data);
        };

        webSocket.OnError += (sender, e) =>
        {
            Debug.LogWarning(@$"Error! {e.Message}");
        };

        // waiting for messages
        webSocket.Connect();
        yield break;
    }

    private IEnumerator MessageProcesser(string inMessage)
    {
        RTCMessage desc = JsonUtility.FromJson<RTCMessage>(inMessage);
        switch(desc.type)
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
            default:
                {
                    RTCSessionDescription session = new RTCSessionDescription { type = RTCSdpType.Offer, sdp = desc.sdp };
                    var op1 = rtcPeerConnection.SetRemoteDescription(ref session);
                    yield return new WaitUntil(() => op1.IsDone);

                    var op2 = rtcPeerConnection.CreateAnswer();
                    yield return new WaitUntil(() => op2.IsDone);

                    var ans_session = op2.Desc;
                    var op3 = rtcPeerConnection.SetLocalDescription(ref ans_session);
                    yield return new WaitUntil(() => op3.IsDone);
                   
                    var data = JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        {"type", ans_session.type.ToString() },
                        {"sdp", ans_session.sdp},
                    });
                    webSocket.Send(data);
                }
                break;
        }
        if (desc.type == "ice")
        {

        }
        else
        {

        }
        yield break;
    }

    private IEnumerator OnCapture()
    {
        yield return new WaitForEndOfFrame();
        
        RenderTexture cam_render_texture = CaptureCamera.targetTexture;
        CaptureCamera.targetTexture = cameraTexture;
        CaptureCamera.Render();
        CaptureCamera.targetTexture = cam_render_texture;

        yield break;
    }
}