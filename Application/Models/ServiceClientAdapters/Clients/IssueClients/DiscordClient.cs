﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.ServiceClientAdapters.Clients.IssueClients
{
    public class DiscordClient : IServiceIssueClient
    {
        public Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthenticated()
        {
            return false;
        }

        public Task<List<IdentifierData>?> GetTicketTypes()
        {
            throw new NotImplementedException();
        }

        public Task<List<IdentifierData>?> GetLabels()
        {
            throw new NotImplementedException();
        }

        public Task<List<IdentifierData>?> GetPriorities()
        {
            throw new NotImplementedException();
        }

        public  Task<List<IdentifierData>?> GetCustomfields()
        {
            throw new NotImplementedException();
        }

        public Task<List<UserData>?> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string?)> Post(TicketData inTicketData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailableFileUpload()
        { 
            return false;
        }

        public void OpenWebBrowser(string inIssueKey)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TicketData> GetIssues()
        {
            throw new NotImplementedException();
        }
    }
}
