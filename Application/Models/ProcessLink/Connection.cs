using BocchiTracker.ProcessLink.ProcessData;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink
{
    public class Connection
    {
        private TcpListener _listener = default!;
        private ConcurrentDictionary<IPAddress, TcpClient> _clients = new ConcurrentDictionary<IPAddress, TcpClient>();
        private readonly IEventAggregator _eventAggregator;
        private IServiceProcessData _serviceProcessData;
        private CancellationTokenSource _cancellationTokenSource;

        public Connection(IEventAggregator inEventAggregator, IServiceProcessData inServiceProcessData)
        {
            _eventAggregator = inEventAggregator;
            _serviceProcessData = inServiceProcessData;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(int inPort)
        {
            _listener = new TcpListener(IPAddress.Any, inPort);
            _listener.Start();
            
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(_cancellationTokenSource.Token);
                    var endPoint = client.Client?.RemoteEndPoint as IPEndPoint;

                    if (endPoint is not null)
                    {
                        if(_clients.ContainsKey(endPoint.Address))
                        {
                            var removeClient = _clients[endPoint.Address];
                            removeClient.Close();
                            _clients.TryRemove(endPoint.Address, out _);
                        }

                        if (_clients.TryAdd(endPoint.Address, client))
                        {
                            _ = HandleClientAsync(endPoint.Address, client);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _listener.Stop();

            // Close all client connections
            foreach (var client in _clients.Values)
            {
                client.Close();
            }
            _clients.Clear();
        }

        private bool IsConnected(TcpClient tcpClient)
        {
            if (!tcpClient.Connected)
            {
                return false;
            }

            if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[1];
                if (tcpClient.Client.Receive(buffer, SocketFlags.Peek) == 0)
                {
                    // ソケットが閉じられている
                    return false;
                }
            }
            return true;
        }

        private async Task HandleClientAsync(IPAddress inIP, TcpClient ioClient)
        {
            Console.WriteLine($"Client connected: {inIP}");
            AppStatusQuery appStatusQuery = new AppStatusQuery(_eventAggregator, _serviceProcessData, inIP.GetHashCode(), ioClient);
            while (IsConnected(ioClient))
            {
                try
                {
                    await appStatusQuery.QueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred with client {inIP}: {ex.Message}");
                    break;
                }
            }
            ioClient.Close();
            _clients.TryRemove(inIP, out _);
            Console.WriteLine($"Client disconnected: {inIP}");
        }
    }
}
