using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ApplicationInfoCollector;

namespace BocchiTracker.CrossServiceReporter
{
    public interface IIssuePoster
    {
        Task<string?> Post(ServiceDefinitions inService, IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig);
    }

    public class IssuePoster : IIssuePoster
    {
        private readonly IServiceClientFactory _clientFactory;
        private readonly ITicketDataFactory _ticketFactory;

        public IssuePoster(IServiceClientFactory inClientFactory, ITicketDataFactory inTicketDataFactory) 
        {
            _clientFactory = inClientFactory;
            _ticketFactory = inTicketDataFactory;
        }

        public async Task<string?> Post(ServiceDefinitions inService, IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig) 
        {
            var client = _clientFactory.CreateIssueService(inService);
            if (client == null)
                return null;

            var ticket = _ticketFactory.Create(inService, inIssueBundle, inAppBundle, inConfig);
            if (ticket == null)
                return null;

            var postResult = await client.Post(ticket);
            if (postResult.Item1 || postResult.Item2 == null)
                return null;

            return postResult.Item2;
        }
    }
}
