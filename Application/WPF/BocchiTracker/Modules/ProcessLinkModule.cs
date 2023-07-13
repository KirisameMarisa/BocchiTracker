using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.ProcessLink.ProcessData;
using Prism.Ioc;
using Prism.Modularity;
using BocchiTracker.ProcessLink;

namespace BocchiTracker.Modules
{
    public class ProcessLinkModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var serviceProcessData = containerProvider.Resolve<IServiceProcessData>();
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.AppBasicInfo, new ProcessDataAppBasicInfo());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.PlayerPosition, new ProcessDataPlayerPosition());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.ScreenshotData, new ProcessDataScreenshotData());

            var configRepo = containerProvider.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = configRepo.Load();
            //!< force exit?
            if (projectConfig == null)
                return;

            //!< Start connection
            var connection = containerProvider.Resolve<Connection>();
            _ = connection.StartAsync(projectConfig.Port);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IServiceProcessData, ServiceProcessData>();
            containerRegistry.RegisterSingleton(typeof(Connection));
        }
    }
}
