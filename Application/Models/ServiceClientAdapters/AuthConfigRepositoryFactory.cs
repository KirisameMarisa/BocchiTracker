using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IAuthConfigRepositoryFactory
    {
        AuthConfig? Load(ServiceDefinitions inServiceType);
        void        Save(ServiceDefinitions inServiceType, AuthConfig inAuthConfig);
    }

    public class AuthConfigRepositoryFactory : IAuthConfigRepositoryFactory
    {
        private static ConcurrentDictionary<ServiceDefinitions, ConfigRepository<AuthConfig>> _caches = new ConcurrentDictionary<ServiceDefinitions, ConfigRepository<AuthConfig>>();

        public AuthConfigRepositoryFactory(string inBaseDirectory)
        {
            foreach (ServiceDefinitions serviceType in Enum.GetValues(typeof(ServiceDefinitions)))
            {
                if (_caches.ContainsKey(serviceType))
                    continue;
                string filePath = Path.Combine(inBaseDirectory, $"{serviceType}.AuthConfig.yaml");
                var configRepo = new ConfigRepository<AuthConfig>(new FileSystem());
                configRepo.SetLoadFilename(filePath);
                _caches.TryAdd(serviceType, configRepo);
            }
        }

        public AuthConfig? Load(ServiceDefinitions inServiceType)
        {
            return _caches[inServiceType].Load();
        }

        public void Save(ServiceDefinitions inServiceType, AuthConfig inAuthConfig)
        {
            _caches[inServiceType].Save(inAuthConfig);
        }
    }
}
