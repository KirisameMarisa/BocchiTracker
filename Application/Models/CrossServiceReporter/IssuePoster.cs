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
using BocchiTracker.ServiceClientAdapters.IssueClients;

namespace BocchiTracker.CrossServiceReporter
{
    public interface IIssuePoster
    {
        void Post(IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig);
    }

    public class IssuePoster : IIssuePoster
    {
        private readonly IServiceIssueClientFactory _client_factory;
        private readonly ITicketDataFactory _ticket_factory;

        public IssuePoster(IServiceIssueClientFactory inClientFactory, ITicketDataFactory inTicketDataFactory) 
        {
            _client_factory = inClientFactory;
            _ticket_factory = inTicketDataFactory;
        }

        public void Post(IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig) 
        {
            foreach(var service in inIssueBundle.IssuePostServices) 
            {
                var client = _client_factory.CreateServiceClientAdapter(service);
                var ticket = _ticket_factory.Create(service, inIssueBundle, inAppBundle, inConfig);
                if (ticket != null)
                {
                    client.Post(ticket);
                }
            }
        }
    }
}
