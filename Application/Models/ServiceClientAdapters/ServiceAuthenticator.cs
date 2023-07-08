using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters
{
    public class ServiceAuthenticator
    {
        private readonly IServiceClientFactory _serviceClientFactory;
        private readonly IAuthConfigRepositoryFactory _authConfigRepositoryFactory;

        public ServiceAuthenticator(IServiceClientFactory inServiceClientFactory, IAuthConfigRepositoryFactory inAuthConfigRepositoryFactory)
        {
            _serviceClientFactory = inServiceClientFactory;
            _authConfigRepositoryFactory = inAuthConfigRepositoryFactory;
        }

        public async Task ReauthenticateServices(List<ServiceConfig> inServiceConfigs)
        {
            foreach (var serviceConfig in inServiceConfigs)
            {
                if (serviceConfig == null)
                    continue;

                if (string.IsNullOrEmpty(serviceConfig.URL))
                    continue;

                var authConfig = _authConfigRepositoryFactory.Load(serviceConfig.Service);
                if (authConfig == null)
                    continue;

                var client = _serviceClientFactory.CreateService(serviceConfig.Service);
                await client.Authenticate(authConfig, serviceConfig.URL, serviceConfig.ProxyURL);
            }
        }
    }
}
