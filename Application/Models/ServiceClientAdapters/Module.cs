using BocchiTracker.ServiceClientAdapters.Data;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.ServiceClientAdapters
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
            containerRegistry.RegisterSingleton<IDescriptionParser, DescriptionParser>();
        }
    }
}
