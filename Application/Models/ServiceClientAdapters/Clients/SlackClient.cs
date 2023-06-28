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

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    public class SlackClient : IServiceClientAdapter
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

        public async Task<bool> Post(TicketData inTicketData)
        {
            if (_client == null)
                return false;

            if (string.IsNullOrEmpty(_channel) || _channel[0] != '#')
                return false;

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
            
            return response.OK;
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetTicketTypes()
#pragma warning restore CS1998
        {
            return null;
        }

#pragma warning disable CS1998
        public async Task<List<IdentifierData>?> GetLabels()
#pragma warning restore CS1998
        {
            return null;
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
