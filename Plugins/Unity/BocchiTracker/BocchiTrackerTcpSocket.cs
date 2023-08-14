using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;

namespace BocchiTracker
{
    public class BocchiTrackerTcpSocket
    {
        public Action<List<byte>> ReciveCallback { private get; set; }

        private Socket socket;
        private Queue<List<byte>> sendDataQueue = new Queue<List<byte>>();

        public BocchiTrackerTcpSocket()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false;
        }

        public async Task Connect(string inIPAddress, int inPort)
        {
            if (IPAddress.TryParse(inIPAddress, out IPAddress ipAddress))
            {
                while(!IsConnect())
                {
                    try
                    {
                        await socket.ConnectAsync(ipAddress, inPort);
                        Debug.Log("Connected to server.");
                    } catch {}
                }
            }
        }

        public void DisConnect()
        {
            socket.Disconnect(true);
        }

        public async Task Update()
        {
            if (sendDataQueue.TryDequeue(out List<byte> data))
            {
                await ProcessSendData(data);
            }
            await ProcessReciveData();
        }

        public void AddSendData(List<byte> inData)
        {
            sendDataQueue.Enqueue(inData);
        }

        private async Task ProcessSendData(List<byte> inData)
        {
            if (!IsConnect())
                return;

            int bytesSent = await socket.SendAsync(inData.ToArray(), SocketFlags.None);
            if (bytesSent > 0)
            {
                Console.WriteLine("Data sent successfully: " + bytesSent + " bytes");
            }
            else
            {
                Console.WriteLine("Failed to send data.");
            }
        }

        private async Task ProcessReciveData()
        {
            if (!IsConnect())
                return;

            byte[] receivedData = new byte[1024];
            int bytesRead = await socket.ReceiveAsync(receivedData, SocketFlags.None);
            if (bytesRead > 0)
            {
                Console.WriteLine("ProcessReciveData::Success, size=" + bytesRead);
                List<byte> receivedDataList = new List<byte>(receivedData);
                receivedDataList.RemoveRange(bytesRead, receivedData.Length - bytesRead);
                ReciveCallback?.Invoke(receivedDataList);
            }
        }

        public bool IsConnect()
        {
            return socket != null && socket.Connected;
        }
    }
}
