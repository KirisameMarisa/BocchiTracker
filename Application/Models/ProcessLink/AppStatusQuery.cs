using BocchiTracker.ModelEventBus;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
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
    public class AppStatusQuery : IDisposable, IRequestHandler<RequestQueryEvent>
    {
        private readonly TcpClient _tcpClient;
        private readonly int _clientId;
        private readonly IMediator _mediator;
        private readonly IServiceProcessData _serviceProcessData;

        public AppStatusQuery(IMediator inMediator, IServiceProcessData inServiceProcessData, int inClientID, TcpClient inClient)
        {
            _clientId = inClientID;
            _tcpClient = inClient;
            _mediator = inMediator;
            _serviceProcessData = inServiceProcessData;
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

                await _serviceProcessData.Process(_mediator, _clientId, root);
            }
        }

        public Task Handle(RequestQueryEvent request, CancellationToken cancellationToken)
        {
            if (_clientId != request.ClientID)
                return Task.CompletedTask;

            var fbb = new FlatBufferBuilder(1024);
            RequestQuery.StartRequestQuery(fbb);
            RequestQuery.AddQueryId(fbb, (int)request.QueryID);
            var table = RequestQuery.EndRequestQuery(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, QueryID.RequestQuery);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);

            _tcpClient.Client.Send(fbb.SizedByteArray());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _mediator.Send(new ModelEventBus.AppDisconnectEvent{ ClientID = _clientId });
        }
    }
}
