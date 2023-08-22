using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ServiceClientData.Configs;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using Prism.Ioc;
using Prism.Modularity;
using System.IO;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.Client.Share.Modules
{
    public class ServiceClientAdaptersModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICacheProvider, CacheProvider>();
            containerRegistry.RegisterSingleton<IDataRepository, DataRepository>();
            containerRegistry.RegisterSingleton<IServiceClientFactory, ServiceClientAdapterFactory>();
            containerRegistry.RegisterSingleton<IPasswordService, PasswordService>();
            containerRegistry.RegisterSingleton<IAuthConfigRepositoryFactory, AuthConfigRepositoryFactory>();
        }
    }
}
