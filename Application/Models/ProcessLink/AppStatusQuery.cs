using BocchiTracker.ModelEventBus;
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
        private TcpClient _tcp_client;
        private readonly int _client_id;
        private readonly IMediator _mediator;

        public AppStatusQuery(IMediator inMediator, int inClientID, TcpClient inClient)
        {
            _client_id = inClientID;
            _tcp_client = inClient;
            _mediator = inMediator;
        }

        public async Task QueryAsync()
        {
            using (NetworkStream stream = _tcp_client.GetStream())
            {
                var buffer_size = new byte[4];
                await stream.ReadAsync(buffer_size, 0, 4);
                int length = BitConverter.ToInt32(buffer_size, 0);
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

                var process_data_func = ProcessData.ProcessDataFactory.Create(root);

                if (process_data_func is not null)
                    await process_data_func.Process(_mediator, _client_id);
            }
        }

        public Task Handle(RequestQueryEvent request, CancellationToken cancellationToken)
        {
            if (_client_id != request.ClientID)
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

            _tcp_client.Client.Send(fbb.SizedByteArray());
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _mediator.Send(new ModelEventBus.AppDisconnectEvent{ ClientID = _client_id });
        }
    }
}
