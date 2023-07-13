using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ApplicationInfoCollector;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.Modules
{
    public class ApplicationInfoCollectorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) 
        {
            containerProvider.Resolve<AppStatusQueryHandler>();
            containerProvider.Resolve<AppDisconnectHandler>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var appStatusBundles = new AppStatusBundles();
            containerRegistry.RegisterInstance(appStatusBundles);
            containerRegistry.RegisterSingleton(typeof(AppStatusQueryHandler));
            containerRegistry.RegisterSingleton(typeof(AppDisconnectHandler));
        }
    }
}
