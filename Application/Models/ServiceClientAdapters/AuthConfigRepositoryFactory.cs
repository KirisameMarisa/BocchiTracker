using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.Config;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IAuthConfigRepositoryFactory
    {
        void Initialize(string inBaseDirectory);
        AuthConfig? Load(ServiceDefinitions inServiceType);
        void Save(ServiceDefinitions inServiceType, AuthConfig inAuthConfig);
    }

    public class AuthConfigRepositoryFactory : IAuthConfigRepositoryFactory
    {
        private static ConcurrentDictionary<ServiceDefinitions, ConfigRepository<AuthConfig>> _caches = new ConcurrentDictionary<ServiceDefinitions, ConfigRepository<AuthConfig>>();
        private IPasswordService _passwordService;

        public AuthConfigRepositoryFactory(IPasswordService inPasswordService)
        {
            _passwordService = inPasswordService;
        }

        public void Initialize(string inBaseDirectory)
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
            if (!_caches.ContainsKey(inServiceType))
                return null;

            AuthConfig? authConfig = null;
            bool result = _caches[inServiceType].TryLoad(out authConfig);

            if (result && authConfig != null)
            {
                if (!string.IsNullOrEmpty(authConfig.APIKey))
                    authConfig.APIKey = _passwordService.Decrypy(authConfig.APIKey);
                if (!string.IsNullOrEmpty(authConfig.Password))
                    authConfig.Password = _passwordService.Decrypy(authConfig.Password);
            }
            return authConfig;
        }

        public void Save(ServiceDefinitions inServiceType, AuthConfig inAuthConfig)
        {
            if (!string.IsNullOrEmpty(inAuthConfig.APIKey))
                inAuthConfig.APIKey = _passwordService.Encrypy(inAuthConfig.APIKey);
            if (!string.IsNullOrEmpty(inAuthConfig.Password))
                inAuthConfig.Password = _passwordService.Encrypy(inAuthConfig.Password);
            _caches[inServiceType].Save(inAuthConfig);
        }
    }
}
