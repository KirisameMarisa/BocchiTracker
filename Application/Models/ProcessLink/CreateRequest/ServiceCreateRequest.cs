using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ProcessLink.CreateRequest
{
    public interface IServiceCreateRequest
    {
        void Register(QueryID inQueryID, ICreateRequest inProcessData);

        byte[]? Create(RequestEventParameterBase inRequest);
    }

    public class ServiceCreateRequest : IServiceCreateRequest
    {
        private Dictionary<QueryID, List<ICreateRequest>> _requests = new Dictionary<QueryID, List<ICreateRequest>>();

        public byte[]? Create(RequestEventParameterBase inRequest)
        {
            if (_requests.ContainsKey(inRequest.QueryID))
            {
                foreach (var r in _requests[inRequest.QueryID])
                {
                    return r.Create(inRequest);
                }
            }
            return null;
        }

        public void Register(QueryID inQueryID, ICreateRequest inProcessData)
        {
            if (!_requests.ContainsKey(inQueryID))
                _requests.Add(inQueryID, new List<ICreateRequest>());

            _requests[inQueryID].Add(inProcessData);
        }
    }
}
