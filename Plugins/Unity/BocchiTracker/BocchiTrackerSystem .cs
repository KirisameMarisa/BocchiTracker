using System;
using UnityEditor;
using UnityEngine;
using BocchiTracker;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Google.FlatBuffers;

namespace BocchiTracker
{
    public class BocchiTrackerSystem : MonoBehaviour
    {
        private static BocchiTrackerTcpSocket tcpSocket;
        private BocchiTrackerSetting setting;
        private bool isSentAppBasicInfo;
        private Queue<ProcessLinkQuery.Queries.QueryID> pendingProcessRequest 
            = new Queue<ProcessLinkQuery.Queries.QueryID>();

        public BocchiTrackerSystem()
        {
            if(tcpSocket == null)
                tcpSocket = new BocchiTrackerTcpSocket();
        }

        async void Start()
        {
            setting = GetComponent<BocchiTrackerSetting>();
            if (!tcpSocket.IsConnect())
            {
                isSentAppBasicInfo = false;
                await tcpSocket.Connect(setting.ServerAddress, setting.ServerPort);
            }
            tcpSocket.ReciveCallback = this.OnReceiveData;
        }

        void OnDestory()
        {
            tcpSocket.DisConnect();
        }

        async void Update()
        {
            if (tcpSocket.IsConnect())
            {
                if (!isSentAppBasicInfo)
                    ProcessSendAppBasicInfo();
                await tcpSocket.Update();
            }
        }

        private void LateUpdate()
        {
            if (tcpSocket.IsConnect())
            {
                if (pendingProcessRequest.TryDequeue(out ProcessLinkQuery.Queries.QueryID OutQueryID))
                {
                    switch (OutQueryID)
                    {
                        case ProcessLinkQuery.Queries.QueryID.ScreenshotData:
                            ProcessSendScreenshot(); break;
                        default: break;
                    }
                }
            }
        }

        void OnReceiveData(List<byte> inData)
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

        public void BocchiTrackerSendPacket(List<byte> inData)
        {
            tcpSocket.AddSendData(inData);
        }

        void ProcessSendAppBasicInfo()
        {
            var appBasicInformationPacket = CreatePacketHelper.CreateApplicationBasicInformation();
            BocchiTrackerSendPacket(appBasicInformationPacket);

            isSentAppBasicInfo = true;
        }

        void ProcessSendScreenshot()
        {
            if (!tcpSocket.IsConnect())
                return;

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            Texture2D screenshotTexture = new Texture2D(screenWidth, screenHeight, TextureFormat.RGBA32, false);
            screenshotTexture.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            screenshotTexture.Apply();

            byte[] screenshotData = screenshotTexture.EncodeToPNG();

            var screenshotDataPacket = CreatePacketHelper.CreateScreenshotData(screenWidth, screenHeight, screenshotData);
            BocchiTrackerSendPacket(screenshotDataPacket);
        }
    }
}
