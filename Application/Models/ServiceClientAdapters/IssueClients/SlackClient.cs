using Slack.NetStandard;
using Slack.NetStandard.WebApi.Chat;
using Slack.NetStandard.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientAdapters.UploadClients;

namespace BocchiTracker.ServiceClientAdapters.IssueClients
{
    public class SlackClient : IServiceIssueClient, IServiceUploadClient
    {
        private SlackWebApiClient? _client;
        private string? _channel;

        public async Task<bool> Authenticate(AuthConfig inAuthConfig, string inURL, string? inProxyURL = null)
        {
            try
            {
                _channel = inURL;
                var token = inAuthConfig.APIKey;
                _client = new SlackWebApiClient(token);
                var result = await _client.Auth.Test();
                return result.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool, string?)> Post(TicketData inTicketData)
        {
            if (_client == null)
                return (false, null);

            if (string.IsNullOrEmpty(_channel) || _channel[0] != '#')
                return (false, null);

            var text = string.Empty;
            text += $"{inTicketData.Summary}\n{inTicketData.Description}";
            if(!string.IsNullOrEmpty(inTicketData.Assignee))
            {
                text += $"\n\n <@{inTicketData.Assignee}>";
            }

            var response = await _client.Chat.Post(new PostMessageRequest
            {
                Channel = _channel,
                Text    = text
            });
            
            return (response.OK, "success");
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            throw new NotImplementedException();
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetTicketTypes()
        {
            return null;
        }
#pragma warning restore CS1998

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetLabels()
        {
            return null;
        }
#pragma warning restore CS1998


#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetPriorities()
        {
            return null;
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
    }
}
