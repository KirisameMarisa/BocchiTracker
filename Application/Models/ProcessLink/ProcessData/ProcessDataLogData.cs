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
            var logData = new List<string>();
            
            for (int i = 0; i < data.LogLength; ++i)
                logData.Add(data.Log(i));

            inEventAggregator
                .GetEvent<ReceiveLogDataEvent>()
                .Publish(new ReceiveLogDataEventParameter(logData));
        }
    }
}
