using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using BocchiTracker;
using BocchiTracker.Views;
using Prism.Regions;
using Slack.NetStandard.Objects;
using BocchiTracker.ViewModels;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.CrossServiceReporter.Converter;
using System.IO.Abstractions;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ApplicationInfoCollector;
using System.Threading.Tasks;
using System.Diagnostics;
using Unity;
using System.IO;
using System.Reflection;
using System;
using Prism.Events;
using BocchiTracker.Event;
using Unity.Resolution;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLink;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.IssueAssetCollector.Handlers;
using Prism.Modularity;
using BocchiTracker.Modules;
using BocchiTracker.Data;

namespace BocchiTracker.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() 
        { 
            return Container.Resolve<MainWindow>(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var connection = Container.Resolve<Connection>();
            connection.Stop();

            base.OnExit(e);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TicketBasicRegion", typeof(TicketBasicView));
            regionManager.RegisterViewWithRegion("TicketDetailsRegion", typeof(TicketDetailsView));
            regionManager.RegisterViewWithRegion("UtilityRegion", typeof(UtilityView));
            regionManager.RegisterViewWithRegion("UploadFilesRegion", typeof(UploadFilesView));

            var projectConfig               = LoadProjectConfig(Container);
            //!< force exit?
            if (projectConfig == null)
                return;

            var dataRepository              = Container.Resolve<IDataRepository>();
            var issueInfoBundle             = Container.Resolve<IssueInfoBundle>();
            var serviceClientFactory        = Container.Resolve<IServiceClientFactory>();
            var autoConfigRepositoryFactory = Container.Resolve<IAuthConfigRepositoryFactory>();

            Task.Run(async () =>
            {
                var serviceAuthenticator = new ServiceAuthenticator(serviceClientFactory, autoConfigRepositoryFactory);
                await Task.Run(() => serviceAuthenticator.ReauthenticateServices(projectConfig.ServiceConfigs));
                await issueInfoBundle.Initialize(dataRepository);
                PublishEvents(Container, projectConfig);
            });
        }

        private void PublishEvents(IContainerProvider inContainer, ProjectConfig inProjectConfig)
        {
            var eventAggregator = inContainer.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<IssueInfoLoadCompleteEvent>().Publish();
            eventAggregator.GetEvent<ConfigReloadEvent>().Publish(new ConfigReloadEventParameter(inProjectConfig));
        }

        private ProjectConfig LoadProjectConfig(IContainerProvider container)
        {
            var configRepo = container.Resolve<CachedConfigRepository<ProjectConfig>>();
            return configRepo.Load();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(TicketProperty));
            containerRegistry.Register<IFileSystem, FileSystem>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ConfigModule>();
            moduleCatalog.AddModule<ApplicationInfoCollectorModule>();
            moduleCatalog.AddModule<ProcessLinkModule>();
            moduleCatalog.AddModule<ServiceClientAdaptersModule>(
                dependsOn: new string[]
                {
                    typeof(ConfigModule).Name
                });
            moduleCatalog.AddModule<IssueAssetCollectorModule>(
                dependsOn: new string[]
                {
                    typeof(ConfigModule).Name,
                    typeof(ApplicationInfoCollectorModule).Name
                });
            moduleCatalog.AddModule<IssueInfoCollectorModule>(
                dependsOn: new string[]
                {
                    typeof(ApplicationInfoCollectorModule).Name,
                    typeof(ServiceClientAdaptersModule).Name
                });
            moduleCatalog.AddModule<CrossServiceReporterModule>(
                dependsOn: new string[]
                {
                    typeof(ApplicationInfoCollectorModule).Name,
                    typeof(IssueInfoCollectorModule).Name,
                    typeof(IssueAssetCollectorModule).Name,
                    typeof(ServiceClientAdaptersModule).Name,
                });
            moduleCatalog.AddModule<CrossServiceUploaderModule>(
                dependsOn: new string[]
                {
                    typeof(ConfigModule).Name,
                    typeof(IssueAssetCollectorModule).Name,
                    typeof(ServiceClientAdaptersModule).Name,
                });
        }
    }
}
