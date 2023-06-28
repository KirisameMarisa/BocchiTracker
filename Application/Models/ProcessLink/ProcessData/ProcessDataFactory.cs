using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public static class ProcessDataFactory
    {
        static public IProcessData? Create(Packet inPacket)
        {
            if (inPacket.QueryIdType == QueryID.AppBasicInfo)
                return new ProcessDataAppBasicInfo(inPacket.QueryIdAsAppBasicInfo());

            if (inPacket.QueryIdType == QueryID.PlayerPosition)
                return new ProcessDataPlayerPosition(inPacket.QueryIdAsPlayerPosition());

            if (inPacket.QueryIdType == QueryID.ScreenshotData)
                return new ProcessDataScreenshotData(inPacket.QueryIdAsScreenshotData());

            return null;
        }
    }
}
