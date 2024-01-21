
//!< Copyright (c) 2023 Yuto Arita

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace BocchiTracker
{
    /// <summary>
    /// Manages TCP socket communication for the BocchiTracker system.
    /// </summary>
    public class BocchiTrackerTcpSocket
    {
        public Action<List<byte>> ReciveCallback { private get; set; } // Callback to handle received data

        private Socket socket; // The TCP socket
        private Queue<List<byte>> sendDataQueue = new Queue<List<byte>>(); // Queue for outgoing data

        public BocchiTrackerTcpSocket()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false; // Set socket to non-blocking mode
        }

        /// <summary>
        /// Initiates a connection to the specified IP address and port.
        /// </summary>
        /// <param name="inIPAddress">The IP address to connect to.</param>
        /// <param name="inPort">The port number to connect to.</param>
        public async Task Connect(string inIPAddress, int inPort)
        {
            if (IPAddress.TryParse(inIPAddress, out IPAddress ipAddress))
            {
                while (!IsConnect())
                {
                    try
                    {
                        await socket.ConnectAsync(ipAddress, inPort);
                        Debug.Log("Connected to server.");
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Disconnects the socket.
        /// </summary>
        public void DisConnect()
        {
            if(IsConnect())
                socket.Disconnect(true);
        }

        /// <summary>
        /// Updates the socket, processing send and receive operations.
        /// </summary>
        public async Task Update()
        {
            if (sendDataQueue.TryDequeue(out List<byte> data))
            {
                await ProcessSendData(data);
            }
            await ProcessReceiveData();
        }

        /// <summary>
        /// Adds data to the send queue for transmission.
        /// </summary>
        /// <param name="inData">The data to be sent.</param>
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

        private async Task ProcessReceiveData()
        {
            if (!IsConnect())
                return;

            byte[] receivedData = new byte[1024];
            int bytesRead = await socket.ReceiveAsync(receivedData, SocketFlags.None);
            if (bytesRead > 0)
            {
                Console.WriteLine("ProcessReceiveData::Success, size=" + bytesRead);
                List<byte> receivedDataList = new List<byte>(receivedData);
                receivedDataList.RemoveRange(bytesRead, receivedData.Length - bytesRead);
                ReciveCallback?.Invoke(receivedDataList);
            }
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