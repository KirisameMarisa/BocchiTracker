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
    public class CreateRequestJump : ICreateRequest
    {
        public byte[]? Create(RequestEventParameterBase inRequest)
        {
            var jumpRequestQuery = inRequest as JumpRequestEventParameter;
            if (jumpRequestQuery == null)
                return null;

            var fbb = new FlatBufferBuilder(1024);
            var stage = fbb.CreateString(jumpRequestQuery.Stage);
            var location = Vec3.CreateVec3(fbb, jumpRequestQuery.PosX, jumpRequestQuery.PosY, jumpRequestQuery.PosZ);
            JumpRequest.StartJumpRequest(fbb);
            JumpRequest.AddLocation(fbb, location);
            JumpRequest.AddStage(fbb, stage);
            var table = JumpRequest.EndJumpRequest(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, inRequest.QueryID);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);
            return fbb.SizedByteArray();
        }
    }
}
