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
        IAsyncEnumerable<TicketData> Get(ServiceDefinitions inService, CustomFieldListService inCustomFieldListService);
    }

    public class GetIssues : IGetIssues
    {
        private readonly ICustomFieldsToAppInfoConverter _conveter;
        private readonly IServiceClientFactory _clientFactory;

        public GetIssues(IServiceClientFactory inClientFactory, ICustomFieldsToAppInfoConverter inConveter)
        {
            _conveter = inConveter;
            _clientFactory = inClientFactory;
        }

        public async IAsyncEnumerable<TicketData> Get(ServiceDefinitions inService, CustomFieldListService inCustomFieldListService)
        {
            var client = _clientFactory.CreateIssueService(inService);
            if (client == null)
                yield break;

            if (!client.IsAuthenticated())
                return _issuesCache[inService];

            await foreach (var issue in client.GetIssues())
            {
                if(issue.CustomFields != null)
                    issue.CustomFields = _conveter.Convert(inService, issue.CustomFields, inCustomFieldListService);
                yield return issue;
            }
        }
    }
}
