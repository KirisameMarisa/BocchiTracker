using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Octokit;
using Redmine;
using Redmine.Net.Api;
using System.Diagnostics;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    public class GithubClient : IServiceIssueClient
    {
        private Octokit.GitHubClient? _client;
        private string? _url;
        private long? _repoId;
        private bool _isAuthenticated;
        private IDescriptionParser _descriptionParser = new DescriptionParser();

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
            if (IsAuthenticated())
                return true;

            if (string.IsNullOrEmpty(inAuthConfig.APIKey))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} APIKey is null");
                return false;
            }

            if(string.IsNullOrEmpty(inURL))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} URL is null or empty");
                return false;
            }

            if (!inURL.Contains("https://github.com/"))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} not contains GithubURL");
                return false;
            }

            string? ownerRepository = null, nameRepository = null;

            _url = inURL;
            var uri = new Uri(_url);
            var segments = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries); ;
            if (segments.Length >= 2)
            {
                ownerRepository = segments[segments.Length - 2].Trim('/');
                nameRepository  = segments[segments.Length - 1].Trim('/');
            }

            if (string.IsNullOrEmpty(ownerRepository) || string.IsNullOrEmpty(nameRepository))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} Cannt get repository informations");
                return false;
            }

            _client = new GitHubClient(new ProductHeaderValue("BocchiTracker"));
            _client.Credentials = new Credentials(inAuthConfig.APIKey);

            try
            {
                var repo = await _client.Repository.Get(ownerRepository, nameRepository);
                if (repo == null)
                    return false;

                _repoId = repo.Id;
                _isAuthenticated = true;
                return _isAuthenticated;
            }
            catch (Exception)
            {
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
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _client is null.");
                return (false, null);
            }

            if (_repoId == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _repo_id is null");
                return (false, null);
            }

            var createIssue = new NewIssue(inTicketData.Summary)
            {
                Body = inTicketData.Description,
            };

            if(!string.IsNullOrEmpty(inTicketData.Assign?.Name)) 
                createIssue.Assignees.Add(inTicketData.Assign?.Name);

            if (inTicketData.Labels != null)
            {
                foreach (var value in inTicketData.Labels)
                    createIssue.Labels.Add(value);
            }
            try
            {
                var issue = await _client.Issue.Create(_repoId.Value, createIssue);
                return (issue != null, issue?.Number.ToString());
            }
            catch
            {
                Trace.TraceError($"{ServiceDefinitions.Github} Failed to post");
                return (false, null);
            }
        }

        public async Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            return await Task.FromResult(false);
        }

        public bool IsAvailableFileUpload()
        {
            return false;
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetTicketTypes()
#pragma warning restore CS1998
        {
            return null;
        }

        public async Task<List<IdentifierData>?> GetLabels()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _client is null.");
                return null;
            }

            if (_repoId == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _repo_id is null");
                return null;
            }

            try
            {
                var labels = await _client.Issue.Labels.GetAllForRepository(_repoId.Value);
                var result = new List<IdentifierData>();
                foreach (var value in labels)
                {
                    result.Add(new IdentifierData { Name = value.Name, Id = value.Id.ToString() });
                }
                return result;
            }
            catch
            {
                Trace.TraceError($"{ServiceDefinitions.Github} Cannot get lables.");
                return null;
            }
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetPriorities()
#pragma warning restore CS1998
        {
            return null;
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetCustomfields()
#pragma warning restore CS1998
        {
            return null;
        }

        public async Task<List<UserData>?> GetUsers()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _client is null.");
                return null;
            }

            if (_repoId == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _repo_id is null");
                return null;
            }

            try
            {
                var result = new List<UserData>();
                var issues = await _client.Issue.GetAllForRepository(_repoId.Value);
                foreach (var issue in issues)
                {
                    if (issue.Assignee == null)
                        continue;

                    var userData = await _client.User.Get(issue.Assignee.Login);
                    if (result.Contains(new UserData { Email = userData.Email }))
                        continue;

                    result.Add(new UserData
                    {
                        Email = userData.Email,
                        Id = issue.Assignee.Id.ToString(),
                        Name = issue.Assignee.Login,
                        IconURL = issue.Assignee.AvatarUrl
                    });
                }
                return result;
            }
            catch
            {
                Trace.TraceError($"{ServiceDefinitions.Github} Cannot get users.");
                return null;
            }
        }

        public async IAsyncEnumerable<TicketData> GetIssues()
        {
            if (_client == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _client is null.");
                yield break;
            }

            if (_repoId == null)
            {
                Trace.TraceError($"{ServiceDefinitions.Github} _repo_id is null");
                yield break;
            }

            var issues = await _client.Issue.GetAllForRepository(_repoId.Value);

            foreach(var issue in issues)
            {
                var customFields = _descriptionParser.Parse(issue.Body);

                yield return new TicketData
                {
                    Id = issue.Number.ToString(),
                    Summary = issue.Title,
                    Description = issue.Body,
                    CustomFields = customFields,
                    Assign = new UserData { Name = issue.Assignee?.Login },
                    Labels = issue.Labels.Select(x => x.Name).ToList(),
                    Status = issue.State.StringValue
                };
            }
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
