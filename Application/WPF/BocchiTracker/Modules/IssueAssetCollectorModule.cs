using BocchiTracker.IssueAssetCollector.Handlers;
using BocchiTracker.IssueAssetCollector;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.Modules
{
    public class IssueAssetCollectorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFilenameGenerator, TimestampedFilenameGenerator>();
            containerRegistry.RegisterSingleton(typeof(IssueAssetsBundle));
            containerRegistry.RegisterSingleton<ICreateActionHandler, CreateActionHandler>();
        }
    }
}
