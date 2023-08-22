using BocchiTracker.ServiceClientData.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Clients.UploadClients
{
    public class DropboxClients : IServiceUploadClient
    {
        public Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailableFileUpload()
        {
            return true;
        }

        public bool IsAuthenticated()
        {
            return true;
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
            throw new NotImplementedException();
        }
    }
}
