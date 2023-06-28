using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink
{
    internal class Connection
    {
        private TcpListener _listener;
        private ConcurrentDictionary<IPEndPoint, TcpClient> _clients = new ConcurrentDictionary<IPEndPoint, TcpClient>();
        private readonly IMediator _mediator;

        public Connection(int inPort, IMediator inMediator)
        {
            _listener = new TcpListener(IPAddress.Any, inPort);
            _mediator = inMediator;
        }

        public async Task StartAsync()
        {
            _listener.Start();

            while (true)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    var end_point = client.Client?.RemoteEndPoint;

                    if (end_point is not null)
                    {
                        if (_clients.TryAdd((IPEndPoint)end_point, client))
                        {
                            _ = HandleClientAsync((IPEndPoint)end_point, client); // Fire and forget, intentionally not awaited
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(IPEndPoint inIP, TcpClient ioClient)
        {
            Console.WriteLine($"Client connected: {inIP}");
            AppStatusQuery app_status_query = new AppStatusQuery(_mediator, inIP.GetHashCode(), ioClient);
            while (ioClient.Connected)
            {
                try
                {
                    await app_status_query.QueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred with client {inIP}: {ex.Message}");
                    ioClient.Close();
                    _clients.TryRemove(inIP, out _);
                    Console.WriteLine($"Client disconnected: {inIP}");
                    break;
                }
            }
        }
    }
}
