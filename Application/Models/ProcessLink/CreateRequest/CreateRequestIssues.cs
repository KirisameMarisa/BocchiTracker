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
    public class CreateRequestIssues : ICreateRequest
    {
        public byte[]? Create(RequestEventParameterBase inRequest)
        {
            var issueRequestQuery = inRequest as IssuesRequestEventParameter;
            if (issueRequestQuery == null)
                return null;

            var fbb = new FlatBufferBuilder(1024);
            List<Offset<Issue>> issuesOffset = new List<Offset<Issue>>();
            foreach(var ticket in issueRequestQuery.TicketData)
            {
                if (ticket.CustomFields == null)
                    continue;

                float x = float.NaN, y = float.NaN, z = float.NaN; string stage = string.Empty;
                if(ticket.CustomFields.TryGetValue("PlayerPosition.x", out float outX))
                    x = outX;

                if (ticket.CustomFields.TryGetValue("PlayerPosition.y", out float outY))
                    y = outY;

                if (ticket.CustomFields.TryGetValue("PlayerPosition.z", out float outZ))
                    z = outZ;

                if (ticket.CustomFields.TryGetValue("PlayerPosition.stage", out string outStage))
                    stage = outStage;

                if (x == float.NaN || y == float.NaN || z == float.NaN)
                    continue;

                if (string.IsNullOrEmpty(stage))
                    continue;

                var idOffset = fbb.CreateString(ticket.Id);
                var summaryOffset = fbb.CreateString(ticket.Summary);
                var assigneOffset = fbb.CreateString(ticket.Assign?.Name);
                var statusOffset = fbb.CreateString(ticket.Status);
                var stageOffset = fbb.CreateString(stage);
                var locationOffset = Vec3.CreateVec3(fbb, x, y, z);
                Issue.StartIssue(fbb);
                Issue.AddId(fbb, idOffset);
                Issue.AddSummary(fbb, summaryOffset);
                Issue.AddAssign(fbb, assigneOffset);
                Issue.AddStatus(fbb, statusOffset);
                Issue.AddStage(fbb, stageOffset);
                Issue.AddLocation(fbb, locationOffset);
                issuesOffset.Add(Issue.EndIssue(fbb));
            }

            IssueesRequest.StartIssueesRequest(fbb);
            IssueesRequest.CreateIssuesVector(fbb, issuesOffset.ToArray());
            var table = IssueesRequest.EndIssueesRequest(fbb);

            Packet.StartPacket(fbb);
            Packet.AddQueryIdType(fbb, inRequest.QueryID);
            Packet.AddQueryId(fbb, table.Value);
            var packet = Packet.EndPacket(fbb);
            Packet.FinishPacketBuffer(fbb, packet);

            return fbb.SizedByteArray();
        }
    }
}
