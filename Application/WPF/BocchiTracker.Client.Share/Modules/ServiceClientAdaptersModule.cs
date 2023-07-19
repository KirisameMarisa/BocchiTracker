using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.Config.Configs;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using Prism.Ioc;
using Prism.Modularity;
using System.IO;
using BocchiTracker.Config;

namespace BocchiTracker.Client.Share.Modules
{
    public class ServiceClientAdaptersModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) 
        {
            var cacheProvider = containerProvider.Resolve<ICacheProvider>();
            var configRepo = containerProvider.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = configRepo.Load();
            //!< force exit?
            if (projectConfig == null)
                return;

            cacheProvider.SetCacheDirectory(string.IsNullOrEmpty(projectConfig.CacheDirectory) ? Path.GetTempPath() : projectConfig.CacheDirectory);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICacheProvider, CacheProvider>();
            containerRegistry.RegisterSingleton<IDataRepository, DataRepository>();
            containerRegistry.RegisterSingleton<IServiceClientFactory, ServiceClientAdapterFactory>();
            containerRegistry.RegisterInstance<IAuthConfigRepositoryFactory>(
                new AuthConfigRepositoryFactory(Path.Combine("Configs", nameof(AuthConfig) + "s")));
        }
    }
}
