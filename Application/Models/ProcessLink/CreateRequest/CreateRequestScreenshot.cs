using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.CreateRequest
{
    public class CreateRequestScreenshot : ICreateRequest
    {
        public byte[]? Create(RequestEventParameterBase inRequest)
        {
            var fbb = new FlatBufferBuilder(1024);
            ScreenshotRequest.StartScreenshotRequest(fbb);
            var table = ScreenshotRequest.EndScreenshotRequest(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, inRequest.QueryID);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);

            return fbb.SizedByteArray();
        }
    }
}
