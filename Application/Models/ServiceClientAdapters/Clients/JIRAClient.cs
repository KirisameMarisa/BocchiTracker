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

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    internal class JIRAClient : IServiceClientAdapter
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

        public async Task<bool> Post(TicketData inTicketData)
        {
            if (_client == null)
                return false;

            var new_issue = _client.CreateIssue(inTicketData.Project);
            new_issue.Summary       = inTicketData.Summary;
            new_issue.Description   = inTicketData.Description;
            new_issue.Type          = inTicketData.TicketType;
            new_issue.Priority      = inTicketData.Priority;
            new_issue.Assignee      = inTicketData.Assignee;
            
            if (inTicketData.CustomFields != null)
            {
                foreach (var custom_field in inTicketData.CustomFields)
                {
                    if (custom_field.Values == null)
                        continue;

                    if(custom_field.Values.Count == 1)
                        new_issue.CustomFields.Add(custom_field.Name, custom_field.Values[0]);
                    else
                        new_issue.CustomFields.AddArray(custom_field.Name, custom_field.Values.ToArray());
                }
            }

            if(inTicketData.Lables != null)
            {
                new_issue.Labels.Add(inTicketData.Lables.ToArray());
            }

            await new_issue.SaveChangesAsync();

            if (inTicketData.Watcheres != null)
            {
                foreach (var watcher in inTicketData.Watcheres)
                {
                    await new_issue.AddWatcherAsync(watcher);
                }
            }
            return true;
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
