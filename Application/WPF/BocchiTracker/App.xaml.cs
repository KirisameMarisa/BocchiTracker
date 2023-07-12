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

namespace BocchiTracker.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private string _projectConfigName;
 
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length <= 1)
                _projectConfigName = e.Args[0];
            base.OnStartup(e);
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

            var userConfig                  = LoadUserConfig(Container);
            var projectConfig               = LoadProjectConfig(Container);
            //!< force exit?
            if (projectConfig == null)
                return;

            var dataRepository              = Container.Resolve<IDataRepository>();
            var issueInfoBundle             = Container.Resolve<IssueInfoBundle>();
            var serviceClientFactory        = Container.Resolve<IServiceClientFactory>();
            var autoConfigRepositoryFactory = Container.Resolve<IAuthConfigRepositoryFactory>();
            var connection                  = Container.Resolve<Connection>();

            RegisterProcessData(Container);

            //!< Start connection
            _ = connection.StartAsync(projectConfig.Port);

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

        private UserConfig LoadUserConfig(IContainerProvider container)
        {
            var configRepo = container.Resolve<CachedConfigRepository<UserConfig>>();
            return configRepo.Load();
        }

        private void RegisterProcessData(IContainerProvider container)
        {
            var serviceProcessData = container.Resolve<IServiceProcessData>();
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.AppBasicInfo, new ProcessDataAppBasicInfo());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.PlayerPosition, new ProcessDataPlayerPosition());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.ScreenshotData, new ProcessDataScreenshotData());

            container.Resolve<AppStatusQueryHandler>();
            container.Resolve<AppDisconnectHandler>();
        }

        private string GetProjectConfigFilePath()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFileName = $"{_projectConfigName ?? assemblyName}.{nameof(ProjectConfig)}.yaml";
            return Path.Combine("Configs", nameof(ProjectConfig) + "s", configFileName);
        }

        private string GetUserConfigFilePath()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFileName = $"{assemblyName}.{nameof(UserConfig)}.yaml";
            return Path.Combine("Configs", nameof(UserConfig) + "s", configFileName);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(new CachedConfigRepository<UserConfig>(
                new ConfigRepository<UserConfig>(GetUserConfigFilePath(), new FileSystem())));

            containerRegistry.RegisterInstance(new CachedConfigRepository<ProjectConfig>(
                new ConfigRepository<ProjectConfig>(GetProjectConfigFilePath(), new FileSystem())));

            containerRegistry.RegisterInstance<IAuthConfigRepositoryFactory>(
                new AuthConfigRepositoryFactory(Path.Combine("Configs", nameof(AuthConfig) + "s")));
            
            var userConfig      = LoadUserConfig(Container);
            var projectConfig   = LoadProjectConfig(Container);
            if (projectConfig == null)
                return;

            containerRegistry.RegisterInstance<ICacheProvider>(
                new CacheProvider(string.IsNullOrEmpty(projectConfig.CacheDirectory) ? Path.GetTempPath() : projectConfig.CacheDirectory, new FileSystem()));

            containerRegistry.Register<IFileSystem, FileSystem>();
            containerRegistry.Register<IFilenameGenerator, TimestampedFilenameGenerator>();
            containerRegistry.RegisterSingleton<IServiceProcessData, ServiceProcessData>();
            containerRegistry.RegisterSingleton<IAppInfoToCustomFieldsConverter, AppInfoToCustomFieldsConverter>();
            containerRegistry.RegisterSingleton<IServiceClientFactory, ServiceClientAdapterFactory>();
            containerRegistry.RegisterSingleton<IDataRepository, DataRepository>();
            containerRegistry.RegisterSingleton<ITicketDataFactory, TicketDataFactory>();
            containerRegistry.RegisterSingleton<IIssuePoster, IssuePoster>();
            containerRegistry.RegisterSingleton(typeof(Connection));
            containerRegistry.RegisterSingleton(typeof(IssueInfoBundle));
            containerRegistry.RegisterSingleton(typeof(IssueAssetsBundle));

            var appStatusBundles = new AppStatusBundles();
            containerRegistry.RegisterInstance(appStatusBundles);
            containerRegistry.RegisterSingleton(typeof(AppStatusQueryHandler));
            containerRegistry.RegisterSingleton(typeof(AppDisconnectHandler));
        }
    }
}
