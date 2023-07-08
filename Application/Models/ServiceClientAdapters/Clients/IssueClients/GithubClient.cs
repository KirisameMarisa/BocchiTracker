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
using BocchiTracker.Config;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    public class GithubClient : IServiceIssueClient
    {
        private Octokit.GitHubClient? _client;
        private long? _repoId;
        private bool _isAuthenticated;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string inURL, string? inProxyURL = null)
        {
            if (string.IsNullOrEmpty(inAuthConfig.APIKey))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} APIKey is null");
                return false;
            }

            if (!inURL.Contains("https://github.com/"))
            {
                Trace.TraceError($"{ServiceDefinitions.Github} not contains GithubURL");
                return false;
            }

            string? ownerRepository = null, nameRepository = null;

            var uri = new Uri(inURL);
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

            if(!string.IsNullOrEmpty(inTicketData.Assignee)) 
                createIssue.Assignees.Add(inTicketData.Assignee);

            if (inTicketData.Lables != null)
            {
                foreach (var value in inTicketData.Lables)
                    createIssue.Labels.Add(value);
            }
            try
            {
                var issue = await _client.Issue.Create(_repoId.Value, createIssue);
                return (issue != null, issue?.Id.ToString());
            }
            catch
            {
                Trace.TraceError($"{ServiceDefinitions.Github} Failed to post");
                return (false, null);
            }
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            throw new NotImplementedException();
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
                var users = await _client.Repository.Collaborator.GetAll(_repoId.Value);

                var result = new List<UserData>();
                foreach (var user in users)
                {
                    result.Add(new UserData
                    {
                        Email = user.Email,
                        Id = user.Id.ToString(),
                        Name = user.Login,
                        IconURL = user.AvatarUrl
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
    }
}
