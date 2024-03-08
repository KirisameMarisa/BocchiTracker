
//!< Copyright (c) 2023 Yuto Arita

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.FlatBuffers;
using static UnityEngine.Application;
using UnityEditor.SceneManagement;

namespace BocchiTracker
{
    /// <summary>
    /// Manages communication with the BocchiTracker server and handles data packets.
    /// </summary>
    public class BocchiTrackerSystem : MonoBehaviour
    {
        public Queue<BocchiTrackerLocation> JumpRequest { get; set; } = new Queue<BocchiTrackerLocation>();

        private BocchiTrackerTcpSocket tcpSocket;
        private BocchiTrackerSetting setting;
        private BocchiTrackerLogHook logHook;
        private bool isSentAppBasicInfo;
        private Queue<object> pendingProcessRequest = new Queue<object>();

        private void Awake()
        {
            if (logHook == null)
                logHook = new BocchiTrackerLogHook();
        }

        private void Start()
        {
            setting = GetComponent<BocchiTrackerSetting>();
            tcpSocket = gameObject.AddComponent<BocchiTrackerTcpSocket>();
            var video_capture = gameObject.AddComponent<BocchiTrackerVideoCapture>();

            if (!IsConnect())
                isSentAppBasicInfo = false;
            tcpSocket.ReciveCallback = this.OnReceiveData;
        }

        private void Update()
        {
            if (IsConnect())
            {
                if (!isSentAppBasicInfo)
                    ProcessSendAppBasicInfo();

                ProcessSendLogMessage();
            }
        }

        private void LateUpdate()
        {
            if (IsConnect())
            {
                if (pendingProcessRequest.TryDequeue(out object outQuery))
                {
                    if(outQuery is ProcessLinkQuery.Queries.IssueesRequest)
                    {
                        var request = (ProcessLinkQuery.Queries.IssueesRequest)outQuery;
                        for(int i = 0; i < request.IssuesLength; ++i)
                        {
                            var issue = request.Issues(i);
                            if (issue == null)
                                continue;
                            CacheIssueInfo(issue.Value);
                        }
                    }
                    else if(outQuery is ProcessLinkQuery.Queries.JumpRequest)
                    {
                        var request = (ProcessLinkQuery.Queries.JumpRequest)outQuery;
                        var stage = request.Stage;
                        var location = request.Location;
                        if (location == null)
                            return;
                        JumpLocation(stage, location.Value.X, location.Value.Y, location.Value.Z);
                    }
                    else if(outQuery is ProcessLinkQuery.Queries.ScreenshotRequest)
                    {
                        StartCoroutine(ProcessSendScreenshot());
                    }
                }
            }
        }

        public bool IsConnect()
        {
            return tcpSocket != null && tcpSocket.IsConnect();
        }

        private void OnReceiveData(List<byte> inData)
        {
            ProcessLinkQuery.Queries.Packet packet = ProcessLinkQuery.Queries.Packet.GetRootAsPacket(new ByteBuffer(inData.ToArray()));
            ProcessLinkQuery.Queries.QueryID queryID = packet.QueryIdType;

            switch (queryID)
            {
                case ProcessLinkQuery.Queries.QueryID.ScreenshotRequest:
                    pendingProcessRequest.Enqueue(packet.QueryIdAsScreenshotRequest()); break;
                
                case ProcessLinkQuery.Queries.QueryID.IssueesRequest:
                    pendingProcessRequest.Enqueue(packet.QueryIdAsIssueesRequest()); break;

                case ProcessLinkQuery.Queries.QueryID.JumpRequest:
                    pendingProcessRequest.Enqueue(packet.QueryIdAsJumpRequest()); break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sends a data packet to the BocchiTracker server.
        /// </summary>
        /// <param name="inData">The data packet to send.</param>
        public void BocchiTrackerSendPacket(List<byte> inData)
        {
            tcpSocket.AddSendData(inData);
        }

        private void ProcessSendLogMessage()
        {
            string message;
            if(logHook.GetLogBuffer(out message))
            {
                var logDataPacket = CreatePacketHelper.CreateLogData(message);
                BocchiTrackerSendPacket(logDataPacket);
            }
        }

        private void ProcessSendAppBasicInfo()
        {
            var appBasicInformationPacket = CreatePacketHelper.CreateApplicationBasicInformation();
            BocchiTrackerSendPacket(appBasicInformationPacket);

            isSentAppBasicInfo = true;
        }

        private void CacheIssueInfo(ProcessLinkQuery.Queries.Issue inIssue)
        {
            
        }

        private void JumpLocation(string inStage, float inX, float inY, float inZ)
        {
            if (JumpRequest.Count == 0)
                JumpRequest.Enqueue(new BocchiTrackerLocation { Stage = inStage, Location = new Vector3(inX, inY, inZ) });
        }

        private IEnumerator ProcessSendScreenshot()
        {
            if (!IsConnect())
                yield break;

            yield return new WaitForEndOfFrame();

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // Capture the screenshot
            RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 32);
            RenderTexture prev_rt = setting.CaptureCamera.targetTexture;
            Quaternion prev_rot = setting.CaptureCamera.transform.rotation;
            Texture2D screenShot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGBA32, false);

            // Configure camera for screenshot
            setting.CaptureCamera.targetTexture = rt;
            setting.CaptureCamera.Render();
            setting.CaptureCamera.targetTexture = prev_rt;

            // Read and apply the screenshot data
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            screenShot.Apply();
            RenderTexture.active = null;

            Destroy(rt);
            byte[] screenshotData = screenShot.GetRawTextureData();
            byte[] newTextureRawData = new byte[screenshotData.Length];

            // Repack the screenshot data
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    int srcIndex = (screenHeight - y - 1) * screenWidth * 4 + x * 4;
                    int destIndex = y * screenWidth * 4 + x * 4;

                    newTextureRawData[destIndex] = screenshotData[srcIndex];
                    newTextureRawData[destIndex + 1] = screenshotData[srcIndex + 1];
                    newTextureRawData[destIndex + 2] = screenshotData[srcIndex + 2];
                    newTextureRawData[destIndex + 3] = screenshotData[srcIndex + 3];
                }
            }

            // Create a screenshot data packet and send it
            var screenshotDataPacket = CreatePacketHelper.CreateScreenshotData(screenWidth, screenHeight, newTextureRawData);
            BocchiTrackerSendPacket(screenshotDataPacket);
        }
    }
}