
//!< Copyright (c) 2023 Yuto Arita

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using PlasticPipe.PlasticProtocol.Messages;

namespace BocchiTracker
{
    /// <summary>
    /// Manages TCP socket communication for the BocchiTracker system.
    /// </summary>
    public class BocchiTrackerTcpSocket : MonoBehaviour
    {
        public Action<List<byte>> ReciveCallback { private get; set; } // Callback to handle received data

        private Socket socket; // The TCP socket
        private Queue<List<byte>> sendDataQueue = new Queue<List<byte>>(); // Queue for outgoing data

        public void Start()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false; // Set socket to non-blocking mode

            var setting = GetComponent<BocchiTrackerSetting>();
            StartCoroutine(Connect(setting.ServerAddress, setting.ServerPort));
        }

        /// <summary>
        /// Initiates a connection to the specified IP address and port.
        /// </summary>
        /// <param name="inIPAddress">The IP address to connect to.</param>
        /// <param name="inPort">The port number to connect to.</param>
        private IEnumerator Connect(string inIPAddress, int inPort)
        {
            if (IPAddress.TryParse(inIPAddress, out IPAddress ipAddress))
            {
                Task connectTask = socket.ConnectAsync(ipAddress, inPort);
                yield return new WaitUntil(() => connectTask.IsCompleted);
            }
            yield break;
        }

        private void OnDestroy()
        {
            if (IsConnect())
            {
                socket.Disconnect(true);
            }
        }

        /// <summary>
        /// Updates the socket, processing send and receive operations.
        /// </summary>
        public void Update()
        {
            if (sendDataQueue.TryDequeue(out List<byte> data))
            {
                StartCoroutine(ProcessSendData(data));
            }
            StartCoroutine(ProcessReceiveData());
        }

        /// <summary>
        /// Adds data to the send queue for transmission.
        /// </summary>
        /// <param name="inData">The data to be sent.</param>
        public void AddSendData(List<byte> inData)
        {
            sendDataQueue.Enqueue(inData);
        }

        private IEnumerator ProcessSendData(List<byte> inData)
        {
            if (!IsConnect())
                yield break;

            var task = socket.SendAsync(inData.ToArray(), SocketFlags.None);
            yield return new WaitUntil(() => task.IsCompleted);
        }

        private IEnumerator ProcessReceiveData()
        {
            if (!IsConnect())
                yield break;

            byte[] receivedData = new byte[1024];
            var task = socket.ReceiveAsync(receivedData, SocketFlags.None);
            yield return new WaitUntil(() => task.IsCompleted);
            
            try
            {
                int bytesRead = task.Result;
                if (task.Result > 0)
                {
                    Console.WriteLine("ProcessReceiveData::Success, size=" + bytesRead);
                    List<byte> receivedDataList = new List<byte>(receivedData);
                    receivedDataList.RemoveRange(bytesRead, receivedData.Length - bytesRead);
                    ReciveCallback?.Invoke(receivedDataList);
                }
            }
            catch { }
        }

        /// <summary>
        /// Checks if the socket is currently connected.
        /// </summary>
        /// <returns>True if the socket is connected, otherwise false.</returns>
        public bool IsConnect()
        {
            return socket != null && socket.Connected;
        }
    }
}