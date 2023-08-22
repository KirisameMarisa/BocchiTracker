using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters;

namespace BocchiTracker.CrossServiceReporter
{
    public interface IIssueOpener
    {
        void Open(ServiceDefinitions inService, string inIssueKey);
    }

    public class IssueOpener : IIssueOpener
    {
        private readonly IServiceClientFactory _clientFactory;

        public IssueOpener(IServiceClientFactory inClientFactory)
        {
            _clientFactory = inClientFactory;
        }

        public void Open(ServiceDefinitions inService, string inIssueKey)
        {
            var client = _clientFactory.CreateIssueService(inService);
            if (client == null)
                return;
            client.OpenWebBrowser(inIssueKey);
        }
    }
}
