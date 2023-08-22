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
using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientData.Configs;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    public class RedmineClient : IServiceIssueClient
    {
        private RedmineManager? _client;
        private int? _projectId;
        private string? _url;
        private string? _projectName;
        private bool _isAuthenticated;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
            if (string.IsNullOrEmpty(inURL))
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} URL is null or empty");
                return false;
            }

            inURL = inURL.TrimEnd('/');

            var segments = inURL.Split('/');

            if (segments.Length < 3 || segments[^2] != "projects")
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} Invalid Redmine project URL format. The format should be: http://[base_url]/projects/[project_name]");
                return false;
            }

            _projectName = segments[^1];
            _url = string.Join('/', segments.Take(segments.Length - 2));

            var webProxy = !string.IsNullOrEmpty(inProxyURL)
                ? new WebProxy(inProxyURL, true)
                : null;

            if (inAuthConfig.APIKey != null)
            {
                _client = new RedmineManager(host: _url, apiKey:inAuthConfig.APIKey, mimeFormat: MimeFormat.Json, proxy: webProxy);

            }
            else if(inAuthConfig.Username != null && inAuthConfig.Password != null)
            {
                _client = new RedmineManager(host: _url, login: inAuthConfig.Username, password: inAuthConfig.Password, mimeFormat: MimeFormat.Json, proxy: webProxy);
            }

            if (_client == null)
                return false;

            try
            {
                var projects = await _client.GetObjectsAsync<Project>(null);
                _projectId = projects.Where(c => c.Identifier == _projectName).Select(c => c.Id).FirstOrDefault();
                _isAuthenticated = _projectId != null;
                return _isAuthenticated;
            }
            catch
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} Failed authenticate");
                return false;
            }
        }

        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }

        public async Task<(bool, string?)> Post(TicketData inTicketData)
        {
            if (_client == null)
                return (false, null);

            if (_projectId == null)
                return (false, null);

            Issue newIssue = new Issue
            {
                Subject         = inTicketData.Summary,
                Description     = inTicketData.Description,
                Project         = IdentifiableName.Create<IdentifiableName>(_projectId.Value),
                Tracker         = new IdentifiableName { Name = inTicketData.TicketType },
            };

            int id;

            if(inTicketData.TicketType != null && int.TryParse(inTicketData.TicketType, out id))
            {
                newIssue.Tracker = IdentifiableName.Create<IdentifiableName>(id);
            }

            if(inTicketData.Assign != null && int.TryParse(inTicketData.Assign?.Id, out id))
            {
                newIssue.AssignedTo = IdentifiableName.Create<IdentifiableName>(id);
            }

            if (inTicketData.Priority != null && int.TryParse(inTicketData.Priority, out id))
            {
                newIssue.Priority = IdentifiableName.Create<IdentifiableName>(id);
            }

            if (inTicketData.CustomFields != null && inTicketData.CustomFields.Count != 0)
            {
                newIssue.CustomFields = new List<IssueCustomField>();
                foreach (var (key, values) in inTicketData.CustomFields)
                {
                    if (values == null)
                        continue;

                    if(int.TryParse(key, out id))
                    {
                        var newIssueCustomFiled = IssueCustomField.Create<IssueCustomField>(id);
                        newIssueCustomFiled.Values = new List<CustomFieldValue>();
                        foreach (var value in values)
                        {
                            newIssueCustomFiled.Values.Add(new CustomFieldValue { Info = value });
                        }
                        newIssue.CustomFields.Add(newIssueCustomFiled);

                    }
                }
            }

            if (inTicketData.Lables != null && inTicketData.Lables.Count != 0)
            {
                string? category = inTicketData.Lables.First() ?? null;
                if(category != null)
                {
                    if (int.TryParse(category, out id))
                        newIssue.Category = IdentifiableName.Create<IdentifiableName>(id);
                }
            }

            Issue? createdIssue = null;
            try
            {
                createdIssue = await _client.CreateObjectAsync(newIssue);
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
                return (false, null);
            }

            if (inTicketData.Watchers == null)
                return (true, createdIssue?.Id.ToString());

            if (createdIssue == null)
                return (false, null);
            
            try
            {
                createdIssue.Watchers = new List<Watcher>();
                foreach (var userData in inTicketData.Watchers)
                {
                    if (int.TryParse(userData.Id, out id))
                        await _client.AddWatcherToIssueAsync(createdIssue.Id, id);
                }
                return (true, createdIssue?.Id.ToString());
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    using var reader = new StreamReader(response.GetResponseStream());
                    var errorMessage = await reader.ReadToEndAsync();

                    Trace.TraceError(errorMessage);
                }
                return (false, null);
            }
        }

        public bool IsAvailableFileUpload()
        {
            return true;
        }

        public async Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            if (_client == null)
                return false;

            try
            {
                Issue issue = _client.GetObject<Issue>(inIssueKey, new NameValueCollection { { "include", "attachments" } });
                if (issue.Uploads == null)
                    issue.Uploads = new List<Upload>();

                foreach (var file in inFilenames)
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file);

                    Upload attachment = _client.UploadFile(fileBytes);
                    attachment.FileName = Path.GetFileName(file);
                    issue.Uploads.Add(attachment);
                }
                await _client.UpdateObjectAsync(issue.Id.ToString(), issue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<IdentifierData>?> GetTicketTypes()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_projectName))
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var trackers = await _client.GetObjectsAsync<Tracker>(new NameValueCollection { { "project_id", _projectName } });
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
                Trace.TraceError($"{ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_projectName))
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var categories = await _client.GetObjectsAsync<IssueCategory>(new NameValueCollection { { "project_id", _projectName } });
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
                Trace.TraceError($"{ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_projectName))
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var priorities = await _client.GetObjectsAsync<IssuePriority>(new NameValueCollection { { "project_id", _projectName } });
            if (priorities == null)
                return null;

            var result = new List<IdentifierData>();
            foreach (var value in priorities)
            {
                result.Add(new IdentifierData { Name = value.Name, Id = value.Id.ToString() });
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetCustomfields()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _client is null");
                return null;
            }

            if (string.IsNullOrEmpty(_projectName))
            {
                Trace.TraceError($"{ServiceDefinitions.Redmine} _project_name is null");
                return null;
            }

            var customFields = await _client.GetObjectsAsync<CustomField>(new NameValueCollection { { "project_id", _projectName } });
            if (customFields == null)
                return null;

            var result = new List<IdentifierData>();
            foreach (var value in customFields)
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

        public void OpenWebBrowser(string inIssueKey)
        {
            if (string.IsNullOrEmpty(_url))
                return;

            string issueURL = $"{_url}/issues/{inIssueKey}";
            Process.Start(new ProcessStartInfo
            {
                FileName = issueURL,
                UseShellExecute = true
            });
        }
    }
}
