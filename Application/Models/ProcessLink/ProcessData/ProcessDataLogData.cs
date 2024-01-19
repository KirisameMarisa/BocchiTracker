using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.ProcessData
{
    public class ProcessDataLogData : IProcessData
    {
        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            var data = inPacket.QueryIdAsLogData();
            inEventAggregator
                .GetEvent<ReceiveLogDataEvent>()
                .Publish(new ReceiveLogDataEventParameter(data.Log));
        }
    }
}
