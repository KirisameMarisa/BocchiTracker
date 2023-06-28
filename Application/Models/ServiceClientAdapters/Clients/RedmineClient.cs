using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Redmine;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Exceptions;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using BocchiTracker.ServiceClientAdapters.Data;

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    public class RedmineClient : IServiceClientAdapter
    {
        private RedmineManager? _client;
        private int? _project_id;
        private string? _project_name;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string inURL, string? inProxyURL = null)
        {
            inURL = inURL.TrimEnd('/');

            var segments = inURL.Split('/');

            if (segments.Length < 3 || segments[^2] != "projects")
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} Invalid Redmine project URL format. The format should be: http://[base_url]/projects/[project_name]");
                return false;
            }

            _project_name = segments[^1];
            var base_url = string.Join('/', segments.Take(segments.Length - 2));

            var web_proxy = !string.IsNullOrEmpty(inProxyURL)
                ? new WebProxy(inProxyURL, true)
                : null;

            if (inAuthConfig.APIKey != null)
            {
                _client = new RedmineManager(host: base_url, apiKey:inAuthConfig.APIKey, mimeFormat: MimeFormat.Json, proxy: web_proxy);

            }
            else if(inAuthConfig.Username != null && inAuthConfig.Password != null)
            {
                _client = new RedmineManager(host: base_url, login: inAuthConfig.Username, password: inAuthConfig.Password, mimeFormat: MimeFormat.Json, proxy: web_proxy);
            }

            if (_client == null)
                return false;

            try
            {
                var projects = await _client.GetObjectsAsync<Project>(null);
                _project_id = projects.Where(c => c.Identifier == _project_name).Select(c => c.Id).FirstOrDefault();

                return _project_id != null;
            }
            catch
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} Failed authenticate");
                return false;
            }
        }

        public async Task<bool> Post(TicketData inTicketData)
        {
            if (_client == null)
                return false;

            if (_project_id == null)
                return false;

            Issue new_issue = new Issue
            {
                Subject         = inTicketData.Summary,
                Description     = inTicketData.Description,
                Project         = IdentifiableName.Create<IdentifiableName>(_project_id.Value),
                Tracker         = new IdentifiableName { Name = inTicketData.TicketType },
            };

            int id;

            if(inTicketData.TicketType != null && int.TryParse(inTicketData.TicketType, out id))
            {
                new_issue.Tracker = IdentifiableName.Create<IdentifiableName>(id);
            }

            if(inTicketData.Assignee != null && int.TryParse(inTicketData.Assignee, out id))
            {
                new_issue.AssignedTo = IdentifiableName.Create<IdentifiableName>(id);
            }

            if (inTicketData.Priority != null && int.TryParse(inTicketData.Priority, out id))
            {
                new_issue.Priority = IdentifiableName.Create<IdentifiableName>(id);
            }

            if (inTicketData.Watcheres != null)
            {
                var t = Watcher.Create<Watcher>(0);
                foreach (var name in inTicketData.Watcheres)
                {
                    if(int.TryParse(name, out id))
                        new_issue.Watchers.Add(IdentifiableName.Create<Watcher>(id));
                }
            }

            if (inTicketData.CustomFields != null)
            {
                foreach (var custom_filed in inTicketData.CustomFields)
                {
                    if(custom_filed.Values == null)
                        continue;

                    var new_issue_custom_filed = new IssueCustomField { Name = custom_filed.Name };
                    foreach (var value in custom_filed.Values)
                    {
                        new_issue_custom_filed.Values.Add(new CustomFieldValue { Info = value });
                    }
                    new_issue.CustomFields.Add(new_issue_custom_filed);
                }
            }

            if (inTicketData.Lables != null)
            {
                string? category = inTicketData.Lables.First() ?? null;
                if(category != null)
                {
                    if (int.TryParse(category, out id))
                        new_issue.Category = IdentifiableName.Create<IdentifiableName>(id);
                }
            }

            try
            {
                Issue createdIssue = await _client.CreateObjectAsync(new_issue);
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    using var reader = new StreamReader(response.GetResponseStream());
                    var errorMessage = await reader.ReadToEndAsync();

                    // Log the error message or do something else with it
                    Trace.TraceError(errorMessage);
                }

                return false;
            }
        }

        public async Task<List<IdentifierData>?> GetTicketTypes()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_project_name))
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var trackers = await _client.GetObjectsAsync<Tracker>(new NameValueCollection { { "project_id", _project_name } });
            if (trackers == null)
                return null;

            var result = new List<IdentifierData>();
            foreach (var value in trackers)
            {
                result.Add(new IdentifierData { Name = value.Name, Id = value.Id.ToString() });
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetLabels()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_project_name))
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var categories = await _client.GetObjectsAsync<IssueCategory>(new NameValueCollection { { "project_id", _project_name } });
            if (categories == null)
                return null;

            var result = new List<IdentifierData>();
            foreach (var value in categories)
            {
                result.Add(new IdentifierData { Name = value.Name, Id = value.Id.ToString() });
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetPriorities()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_project_name))
            {
                Trace.TraceError($"{ProjectConfig.ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var Priorities = await _client.GetObjectsAsync<IssuePriority>(new NameValueCollection { { "project_id", _project_name } });
            if (Priorities == null)
                return null;

            var result = new List<IdentifierData>();
            foreach (var value in Priorities)
            {
                result.Add(new IdentifierData { Name = value.Name, Id = value.Id.ToString() });
            }
            return result;
        }

        public async Task<List<UserData>?> GetUsers()
        {
            if (_client == null)
                return null;

            var parameters = new NameValueCollection();
            var users = await _client.GetObjectsAsync<User>(parameters);

            var result = new List<UserData>();
            foreach ( var user in users)
            {
                result.Add(new UserData
                {
                    Email = user.Email,
                    Id = user.Id.ToString(),
                    Name = $"{user.FirstName} {user.LastName}",
                    IconURL = null
                });
            }
            return result;
        }
    }
}
