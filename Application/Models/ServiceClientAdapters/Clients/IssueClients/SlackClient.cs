using Slack.NetStandard;
using Slack.NetStandard.WebApi.Chat;
using Slack.NetStandard.Socket;
using Slack.NetStandard.WebApi.Files;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;
using System.Threading;
using System.IO.Abstractions;
using System.Diagnostics;
using System.IO;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    public class SlackClient : IServiceIssueClient
    {
        private SlackWebApiClient? _client;
        private string? _channel;
        private bool _isAuthenticated;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
            if (IsAuthenticated())
                return true;

            if (string.IsNullOrEmpty(inURL))
            {
                Trace.TraceError($"{ServiceDefinitions.Slack} URL is null or empty");
                return false;
            }

            try
            {
                _channel = inURL;
                var token = inAuthConfig.APIKey;
                _client = new SlackWebApiClient(token);
                var result = await _client.Auth.Test();
                _isAuthenticated = result.OK;
                return _isAuthenticated;
            }
            catch
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
                return (false, null);

            if (string.IsNullOrEmpty(_channel) || _channel[0] != '#')
                return (false, null);

            var text = string.Empty;
            text += $"{inTicketData.Summary}\n{inTicketData.Description}";
            if (!string.IsNullOrEmpty(inTicketData.Assign?.Id))
            {
                text += $"\n\n <@{inTicketData.Assign?.Id}>";
            }

            var response = await _client.Chat.Post(new PostMessageRequest
            {
                Channel = _channel,
                Text = text
            });

            if (!response.OK)
                return (false, string.Empty);

            return (true, response.Timestamp.RawValue);
        }

        public bool IsAvailableFileUpload()
        {
            return true;
        }

        public async Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            if (_client == null)
                return false;

            if (_channel == null)
                return false;

            bool has_error = false;
            Timestamp timestamp = inIssueKey;
            foreach (var file in inFilenames)
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                {
                    string fullPath = Path.GetFullPath(file);
                    var responce = await _client.Files.Upload(new FileUploadRequest
                    {
                        Filename = Path.GetFileName(fullPath),
                        Channels = _channel,
                        ThreadTimestamp = timestamp,
                        File = new MultipartFile(fileStream, fullPath),
                    });
                    if (!responce.OK)
                    {
                        Trace.TraceError(responce.Error);
                        has_error = true;
                    }
                }
            }
            return has_error != true;
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetTicketTypes()
        {
            return null;
        }

        public async Task<List<IdentifierData>?> GetLabels()
        {
            return null;
        }

        public async Task<List<IdentifierData>?> GetPriorities()
        {
            return null;
        }

        public async Task<List<IdentifierData>?> GetCustomfields()
        {
            return null;
        }

        public async IAsyncEnumerable<TicketData> GetIssues()
        {
            yield break;
        }
#pragma warning restore CS1998

        public async Task<List<UserData>?> GetUsers()
        {
            if (_client == null)
                return null;

            var response = await _client.Users.List();
            if (!response.OK)
                return null;

            var result = new List<UserData>();
            foreach (var user in response.Members)
            {
                if (string.IsNullOrEmpty(user.Profile.Email))
                    continue;

                result.Add(new UserData
                {
                    Email = user.Profile.Email,
                    Id = user.ID,
                    Name = user.Name,
                    IconURL = user.Profile.Image32
                });
            }
            return result;
        }

        public void OpenWebBrowser(string inIssueKey)
        {
            //!< nothing
        }
    }
}
