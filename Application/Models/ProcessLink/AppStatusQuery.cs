using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLink.CreateRequest;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink
{
    public class AppStatusQuery : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly int _clientId;
        private readonly IEventAggregator _eventAggregator;
        private readonly SubscriptionToken _subscriptionToken;
        private readonly IServiceProcessData _serviceProcessData;
        private readonly IServiceCreateRequest _serviceCreateReuqest;

        public AppStatusQuery(IEventAggregator inEventAggregator, IServiceProcessData inServiceProcessData, IServiceCreateRequest inServiceCreateRequest, int inClientID, TcpClient inClient)
        {
            _clientId = inClientID;
            _tcpClient = inClient;
            _eventAggregator = inEventAggregator;
            _serviceProcessData = inServiceProcessData;
            _serviceCreateReuqest = inServiceCreateRequest;

            _subscriptionToken = _eventAggregator
                .GetEvent<RequestQueryEvent>()
                .Subscribe(Handle, ThreadOption.BackgroundThread);
        }

        public async Task QueryAsync()
        {
            NetworkStream stream = _tcpClient.GetStream();
            {
                var bufferSize = new byte[4];
                await stream.ReadAsync(bufferSize, 0, 4);
                int length = BitConverter.ToInt32(bufferSize, 0);
                if (length == 0)
                    return;

                var buffer = new byte[length];
                int received = 0;
                while (received < length)
                {
                    received += await stream.ReadAsync(buffer, received, length - received);
                }

                var flatBuffer = new ByteBuffer(buffer);
                var root = Packet.GetRootAsPacket(flatBuffer);

                _serviceProcessData.Process(_eventAggregator, _clientId, root);
            }
        }

        public void Handle(RequestEventParameterBase inRequest)
        {
            if (_clientId != inRequest.ClientID)
                return;

            var data = _serviceCreateReuqest.Create(inRequest);
            if (data == null)
                return;

            int ret = _tcpClient.Client.Send(data);
        }

        public void Dispose()
        {
            _eventAggregator
                .GetEvent<AppDisconnectEvent>()
                .Publish(new AppDisconnectEventParameter(_clientId));
            _eventAggregator
                .GetEvent<RequestQueryEvent>()
                .Unsubscribe(_subscriptionToken);
        }
    }
}
