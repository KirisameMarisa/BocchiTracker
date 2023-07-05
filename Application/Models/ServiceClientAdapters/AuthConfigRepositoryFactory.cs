using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientAdapters.IssueClients;
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
        AuthConfig? Load(IssueServiceDefinitions serviceType);
        void        Save(IssueServiceDefinitions inServiceType, AuthConfig inAuthConfig);
    }

    public class AuthConfigRepositoryFactory : IAuthConfigRepositoryFactory
    {
        private static ConcurrentDictionary<IssueServiceDefinitions, ConfigRepository<AuthConfig>> _caches = new ConcurrentDictionary<IssueServiceDefinitions, ConfigRepository<AuthConfig>>();

        public AuthConfigRepositoryFactory(string inBaseDirectory)
        {
            foreach (IssueServiceDefinitions service_type in Enum.GetValues(typeof(IssueServiceDefinitions)))
            {
                if (_caches.ContainsKey(service_type))
                    continue;
                string filepath = Path.Combine(inBaseDirectory, $"{service_type}.AuthConfig.yaml");
                _caches.TryAdd(service_type, new ConfigRepository<AuthConfig>(filepath, new FileSystem()));
            }
        }

        public AuthConfig? Load(IssueServiceDefinitions inServiceType)
        {
            return _caches[inServiceType].Load();
        }

        public void Save(IssueServiceDefinitions inServiceType, AuthConfig inAuthConfig)
        {
            _caches[inServiceType].Save(inAuthConfig);
        }
    }
}
