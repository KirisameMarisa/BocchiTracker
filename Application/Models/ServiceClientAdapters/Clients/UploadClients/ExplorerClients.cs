using BocchiTracker.ServiceClientData.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Clients.UploadClients
{
    public class ExplorerClients : IServiceUploadClient
    {
        public Task<bool> Authenticate(AuthConfig inAuthConfig, string? inURL, string? inProxyURL = null)
        {
#if WINDOWS
            return Task.FromResult(true);
#else
            return Task.FromResult(false);
#endif
        }

        public bool IsAvailableFileUpload()
        {
            return true;
        }

        public bool IsAuthenticated()
        {
            return false;
        }

        public Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames)
        {
#if WINDOWS
            return Task.FromResult(false);
#else
            return Task.FromResult(false);
#endif
        }
    }
}
