using BocchiTracker.CrossServiceReporter.Converter;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter
{
    public interface IGetIssues
    {
        Task<List<TicketData>> Get(ServiceDefinitions inService, CustomFieldListService inCustomFieldListService);
    }

    public class GetIssues : IGetIssues
    {
        private readonly Dictionary<ServiceDefinitions, List<TicketData>> _issuesCache = new Dictionary<ServiceDefinitions, List<TicketData>>();
        private readonly ICustomFieldsToAppInfoConverter _conveter;
        private readonly IServiceClientFactory _clientFactory;

        public GetIssues(IServiceClientFactory inClientFactory, ICustomFieldsToAppInfoConverter inConveter)
        {
            _conveter = inConveter;
            _clientFactory = inClientFactory;
        }

        public async Task<List<TicketData>> Get(ServiceDefinitions inService, CustomFieldListService inCustomFieldListService)
        {
            if(!_issuesCache.ContainsKey(inService))
                _issuesCache.Add(inService, new List<TicketData>());

            if (_issuesCache[inService].Count > 0)
                return _issuesCache[inService];

            var client = _clientFactory.CreateIssueService(inService);
            if (client == null)
                return _issuesCache[inService];

            await foreach (var issue in client.GetIssues())
            {
                if(issue.CustomFields != null)
                    issue.CustomFields = _conveter.Convert(inService, issue.CustomFields, inCustomFieldListService);
                _issuesCache[inService].Add(issue);
            }
            return _issuesCache[inService];
        }
    }
}
