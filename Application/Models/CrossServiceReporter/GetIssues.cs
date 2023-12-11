using BocchiTracker.Config.Configs;
using BocchiTracker.CrossServiceReporter.Converter;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter
{
    public interface IGetIssues
    {
        Task<List<TicketData>> GetAsync(ServiceConfig inServiceConfig);

        List<TicketData> GetFromCache(ServiceConfig inServiceConfig);
    }

    public class GetIssues : IGetIssues
    {
        private readonly ConcurrentDictionary<ServiceDefinitions, List<TicketData>> _issuesCache = new ConcurrentDictionary<ServiceDefinitions, List<TicketData>>();
        private readonly ICustomFieldsToAppInfoConverter _conveter;
        private readonly IServiceClientFactory _clientFactory;

        public GetIssues(IServiceClientFactory inClientFactory, ICustomFieldsToAppInfoConverter inConveter)
        {
            _conveter = inConveter;
            _clientFactory = inClientFactory;
        }

        public async Task<List<TicketData>> GetAsync(ServiceConfig? inServiceConfig)
        {
            var sEmpty = new List<TicketData>();

            if (inServiceConfig == null)
                return sEmpty;

            var client = _clientFactory.CreateIssueService(inServiceConfig.Service);
            if (client == null)
                return sEmpty;

            if (!client.IsAuthenticated())
                return sEmpty;

            if (!_issuesCache.ContainsKey(inServiceConfig.Service))
            {
                if (!_issuesCache.TryAdd(inServiceConfig.Service, new List<TicketData>()))
                    return sEmpty;
            }

            if (_issuesCache.TryGetValue(inServiceConfig.Service, out List<TicketData>? outTickets) 
                && outTickets != null 
                && outTickets.Count > 0)
                return outTickets;

            await foreach (var issue in client.GetIssues())
            {
                if(issue.CustomFields != null)
                    issue.CustomFields = _conveter.Convert(inServiceConfig, issue.CustomFields);
                _issuesCache[inServiceConfig.Service].Add(issue);
            }

            if (_issuesCache.TryGetValue(inServiceConfig.Service, out List<TicketData>? outResult) && outResult != null)
                return outResult;

            return sEmpty;
        }

        public List<TicketData> GetFromCache(ServiceConfig? inServiceConfig)
        {
            var sEmpty = new List<TicketData>();

            if (inServiceConfig == null)
                return sEmpty;

            var client = _clientFactory.CreateIssueService(inServiceConfig.Service);
            if (client == null)
                return sEmpty;

            if (!client.IsAuthenticated())
                return sEmpty;

            if (!_issuesCache.ContainsKey(inServiceConfig.Service))
            {
                if (!_issuesCache.TryAdd(inServiceConfig.Service, new List<TicketData>()))
                    return sEmpty;
            }

            if (_issuesCache.TryGetValue(inServiceConfig.Service, out List<TicketData>? outTickets)
                && outTickets != null
                && outTickets.Count > 0)
                return outTickets;

            return sEmpty;
        }
    }
}
