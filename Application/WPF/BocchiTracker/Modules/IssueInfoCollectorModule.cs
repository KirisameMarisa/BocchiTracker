using BocchiTracker.IssueInfoCollector;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.Modules
{
    public class IssueInfoCollectorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(IssueInfoBundle));
        }
    }
}
