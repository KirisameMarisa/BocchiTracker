using Atlassian.Jira;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    internal class JIRAClient : IServiceIssueClient
    {
        private Jira? _client;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string inURL, string? inProxyURL = null)
        {
            try
            {
                var settings = inProxyURL != null
                    ? new JiraRestClientSettings { Proxy = new WebProxy(inProxyURL, true) }
                    : null;

                _client = Jira.CreateRestClient(inURL, inAuthConfig.Username, inAuthConfig.Password, settings);
                if (_client == null)
                    return false;
                
                var currentUser = await _client.Users.GetMyselfAsync();
                return currentUser != null;
            }
            catch
            {
                return false;
            }
        }

        public Task<(bool, string?)> Post(TicketData inTicketData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            throw new NotImplementedException();
        }

        public  Task<List<IdentifierData>?> GetTicketTypes()
        {
            throw new NotImplementedException();
        }

        public  Task<List<IdentifierData>?> GetLabels()
        {
            throw new NotImplementedException();
        }

        public  Task<List<IdentifierData>?> GetPriorities()
        {
            throw new NotImplementedException();
        }

        public  Task<List<UserData>?> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
