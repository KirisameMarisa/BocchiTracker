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

namespace BocchiTracker.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private string _projectConfigName;

        private void PrismApplication_Startup(object sender, StartupEventArgs e)
        {
            if(e.Args.Length <= 1) 
                _projectConfigName = e.Args[0];
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var projectConfig               = LoadProjectConfig(Container);
            //!< force exit?
            if (projectConfig == null)
                return;

            var userConfig                  = LoadUserConfig(Container);

            var dataRepository              = Container.Resolve<IDataRepository>();
            var issueInfoBundle             = Container.Resolve<IssueInfoBundle>();
            var serviceClientFactory        = Container.Resolve<IServiceClientFactory>();
            var autoConfigRepositoryFactory = Container.Resolve<IAuthConfigRepositoryFactory>();

            Task.Run(async () =>
            {
                var serviceAuthenticator = new ServiceAuthenticator(serviceClientFactory, autoConfigRepositoryFactory);
                await Task.Run(() => serviceAuthenticator.ReauthenticateServices(projectConfig.ServiceConfigs));
                await issueInfoBundle.Initialize(dataRepository);
                PublishEvents(Container);
            });
        }

        private void PublishEvents(IContainerProvider inContainer)
        {
            var eventAggregator = inContainer.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<IssueInfoLoadCompleteEvent>().Publish();
            eventAggregator.GetEvent<ConfigReloadEvent>().Publish();
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
            var container = new UnityContainer();
            container.AddExtension(new Diagnostic());

            containerRegistry.Register<IFileSystem, FileSystem>();
            containerRegistry.RegisterSingleton<IServiceClientFactory, ServiceClientAdapterFactory>();

            containerRegistry.RegisterInstance<ICacheProvider>(new CacheProvider (Path.GetTempPath(), new FileSystem()));
            containerRegistry.RegisterSingleton<IDataRepository, DataRepository>();

            {
                var name = Assembly.GetExecutingAssembly().GetName().Name;
                containerRegistry.RegisterInstance(new CachedConfigRepository<UserConfig>(
                    new ConfigRepository<UserConfig>(Path.Combine("Configs", nameof(UserConfig) + "s" , $"{name}.{nameof(UserConfig)}.yaml"), new FileSystem())));

                name = string.IsNullOrEmpty(_projectConfigName)
                    ? name
                    : _projectConfigName;
                containerRegistry.RegisterInstance(new CachedConfigRepository<ProjectConfig>(
                    new ConfigRepository<ProjectConfig>(Path.Combine("Configs", nameof(ProjectConfig) + "s", $"{name}.{nameof(ProjectConfig)}.yaml"), new FileSystem())));
            }

            {
                var auto_config_repo_factory = new AuthConfigRepositoryFactory(Path.Combine("Configs", nameof(AuthConfig) + "s"));
                containerRegistry.RegisterInstance<IAuthConfigRepositoryFactory>(auto_config_repo_factory);
            }

            containerRegistry.RegisterSingleton<ITicketDataFactory, TicketDataFactory>();
            containerRegistry.RegisterSingleton<IIssuePoster, IssuePoster>();
            containerRegistry.RegisterSingleton<IAppInfoToCustomFieldsConverter, AppInfoToCustomFieldsConverter>();
            containerRegistry.Register<IPasswordService, PasswordService>();
            containerRegistry.RegisterSingleton(typeof(CachedConfigRepository<>));
            containerRegistry.RegisterSingleton(typeof(ConfigRepository<>));
            containerRegistry.RegisterSingleton(typeof(IssueInfoBundle));
            containerRegistry.RegisterSingleton(typeof(TrackerApplication));
            containerRegistry.RegisterSingleton(typeof(IssueAssetsBundle));
            containerRegistry.Register<IFilenameGenerator, TimestampedFilenameGenerator>();
        }
    }
}
