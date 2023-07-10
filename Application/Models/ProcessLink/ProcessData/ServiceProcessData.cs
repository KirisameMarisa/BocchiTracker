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
    public interface IServiceProcessData
    {
        void Register(QueryID inQueryID, IProcessData inProcessData);

        Task Process(IMediator inMediator, int inClientID, Packet inPacket);
    }

    public class ServiceProcessData : IServiceProcessData
    {
        private Dictionary<QueryID, List<IProcessData>> _processDataMap = new Dictionary<QueryID, List<IProcessData>>();

        public async Task Process(IMediator inMediator, int inClientID, Packet inPacket)
        {
            if(_processDataMap.ContainsKey(inPacket.QueryIdType))
            {
                foreach(var proc in _processDataMap[inPacket.QueryIdType]) 
                {
                    await proc.Process(inMediator, inClientID, inPacket);
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
