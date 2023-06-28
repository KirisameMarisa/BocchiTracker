using BocchiTracker.ProjectConfig;
using BocchiTracker.ServiceClientAdapters.Clients;
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
        AuthConfig? Load(ServiceDefinitions serviceType);
        void        Save(ServiceDefinitions inServiceType, AuthConfig inAuthConfig);
    }

    public class AuthConfigRepositoryFactory : IAuthConfigRepositoryFactory
    {
        private static ConcurrentDictionary<ServiceDefinitions, AuthConfigRepository> _caches = new ConcurrentDictionary<ServiceDefinitions, AuthConfigRepository>();

        public AuthConfigRepositoryFactory(string inBaseDirectory)
        {
            foreach (ServiceDefinitions service_type in Enum.GetValues(typeof(ServiceDefinitions)))
            {
                if (_caches.ContainsKey(service_type))
                    continue;
                string filepath = Path.Combine(inBaseDirectory, $"{service_type}.AuthConfig.yaml");
                _caches.TryAdd(service_type, new AuthConfigRepository(filepath, new FileSystem()));
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
