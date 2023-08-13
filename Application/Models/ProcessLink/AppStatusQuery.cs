using BocchiTracker.ModelEvent;
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
        private readonly IServiceProcessData _serviceProcessData;

        public AppStatusQuery(IEventAggregator inEventAggregator, IServiceProcessData inServiceProcessData, int inClientID, TcpClient inClient)
        {
            _clientId = inClientID;
            _tcpClient = inClient;
            _eventAggregator = inEventAggregator;
            _serviceProcessData = inServiceProcessData;

            _eventAggregator
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

        public void Handle(RequestQueryEventParameter request)
        {
            if (_clientId != request.ClientID)
                return;

            var fbb = new FlatBufferBuilder(1024);
            RequestQuery.StartRequestQuery(fbb);
            RequestQuery.AddQueryId(fbb, (int)request.QueryID);
            var table = RequestQuery.EndRequestQuery(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, QueryID.RequestQuery);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);

            int ret = _tcpClient.Client.Send(fbb.SizedByteArray());
            System.Console.WriteLine(ret);
        }

        public void Dispose()
        {
            _eventAggregator
                .GetEvent<AppDisconnectEvent>()
                .Publish(new AppDisconnectEventParameter(_clientId));
        }
    }
}
