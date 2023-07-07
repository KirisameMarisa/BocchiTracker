using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    public interface IService 
    {
        Task<bool> Authenticate(AuthConfig inAuthConfig, string inURL, string? inProxyURL = null);
    }
}
