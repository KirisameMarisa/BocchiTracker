
//!< Copyright (c) 2023 Yuto Arita

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.FlatBuffers;

namespace BocchiTracker
{
    /// <summary>
    /// Manages communication with the BocchiTracker server and handles data packets.
    /// </summary>
    public class BocchiTrackerSystem : MonoBehaviour
    {
        private static BocchiTrackerTcpSocket tcpSocket;
        private BocchiTrackerSetting setting;
        private bool isSentAppBasicInfo;
        private Queue<ProcessLinkQuery.Queries.QueryID> pendingProcessRequest
            = new Queue<ProcessLinkQuery.Queries.QueryID>();

        public BocchiTrackerSystem()
        {
            if (tcpSocket == null)
                tcpSocket = new BocchiTrackerTcpSocket();
        }

        private async void Start()
        {
            setting = GetComponent<BocchiTrackerSetting>();
            if (!IsConnect())
            {
                isSentAppBasicInfo = false;
                await tcpSocket.Connect(setting.ServerAddress, setting.ServerPort);
            }
            tcpSocket.ReciveCallback = this.OnReceiveData;
        }

        private void OnDestroy()
        {
            tcpSocket.DisConnect();
        }

        private async void Update()
        {
            if (IsConnect())
            {
                if (!isSentAppBasicInfo)
                    ProcessSendAppBasicInfo();
                await tcpSocket.Update();
            }
        }

        private void LateUpdate()
        {
            if (IsConnect())
            {
                if (pendingProcessRequest.TryDequeue(out ProcessLinkQuery.Queries.QueryID outQueryID))
                {
                    switch (outQueryID)
                    {
                        case ProcessLinkQuery.Queries.QueryID.ScreenshotData:
                            StartCoroutine(ProcessSendScreenshot());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public bool IsConnect()
        {
            return tcpSocket.IsConnect();
        }

        private void OnReceiveData(List<byte> inData)
        {
            ProcessLinkQuery.Queries.Packet packet = ProcessLinkQuery.Queries.Packet.GetRootAsPacket(new ByteBuffer(inData.ToArray()));
            ProcessLinkQuery.Queries.QueryID queryID = packet.QueryIdType;

            switch (queryID)
            {
                case ProcessLinkQuery.Queries.QueryID.RequestQuery:
                    {
                        ProcessLinkQuery.Queries.RequestQuery requestQuery = packet.QueryIdAsRequestQuery();
                        pendingProcessRequest.Enqueue((ProcessLinkQuery.Queries.QueryID)requestQuery.QueryId);
                        break;
                    }
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

        private void ProcessSendAppBasicInfo()
        {
            var appBasicInformationPacket = CreatePacketHelper.CreateApplicationBasicInformation();
            BocchiTrackerSendPacket(appBasicInformationPacket);

            isSentAppBasicInfo = true;
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
            RenderTexture prev_rt = setting.ScreenshotCamera.targetTexture;
            Quaternion prev_rot = setting.ScreenshotCamera.transform.rotation;
            Texture2D screenShot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGBA32, false);

            // Configure camera for screenshot
            setting.ScreenshotCamera.targetTexture = rt;
            setting.ScreenshotCamera.Render();
            setting.ScreenshotCamera.targetTexture = prev_rt;

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