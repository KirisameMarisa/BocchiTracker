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
    public interface IServiceProcessData
    {
        void Register(QueryID inQueryID, IProcessData inProcessData);

        void Process(IEventAggregator inMediator, int inClientID, Packet inPacket);
    }

    public class ServiceProcessData : IServiceProcessData
    {
        private Dictionary<QueryID, List<IProcessData>> _processDataMap = new Dictionary<QueryID, List<IProcessData>>();

        public void Process(IEventAggregator inEventAggregator, int inClientID, Packet inPacket)
        {
            if(_processDataMap.ContainsKey(inPacket.QueryIdType))
            {
                foreach(var proc in _processDataMap[inPacket.QueryIdType]) 
                {
                    proc.Process(inEventAggregator, inClientID, inPacket);
                }
            }
        }

        public void Register(QueryID inQueryID, IProcessData inProcessData)
        {
            if (!_processDataMap.ContainsKey(inQueryID))
                _processDataMap.Add(inQueryID, new List<IProcessData>());

            _processDataMap[inQueryID].Add(inProcessData);
        }
    }
}
