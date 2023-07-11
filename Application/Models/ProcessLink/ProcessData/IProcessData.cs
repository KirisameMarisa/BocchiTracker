using BocchiTracker.ProcessLinkQuery.Queries;
using Google.FlatBuffers;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public interface IProcessData
    {
        void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket);
    }
}
