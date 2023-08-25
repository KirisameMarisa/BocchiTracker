using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using BocchiTracker;
using BocchiTracker.Client.Views;
using Prism.Regions;
using Slack.NetStandard.Objects;
using BocchiTracker.Client.ViewModels;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.CrossServiceReporter.Converter;
using System.IO.Abstractions;
using BocchiTracker.ServiceClientData;
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
using Unity.Resolution;
using BocchiTracker.ProcessLink.ProcessData;
using BocchiTracker.ProcessLink;
using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.IssueAssetCollector.Handlers;
using Prism.Modularity;
using BocchiTracker.Data;
using BocchiTracker.Client.Share.Controls;
using Prism.Services.Dialogs;
using System.Linq;
using BocchiTracker.Config;
using BocchiTracker.CrossServiceUploader;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        public string ProjectConfigDirectory => Path.Combine("Configs", "ProjectConfigs");

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
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<StartProgressEvent>().Publish(new ProgressEventParameter {});

            var projectConfigRepo = Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var userConfig = LoadUserConfig(Container);
            if (
                    userConfig != null 
                &&  !string.IsNullOrEmpty(userConfig.ProjectConfigFilename) 
                &&  System.IO.File.Exists(userConfig.ProjectConfigFilename)) 
            {
                projectConfigRepo.SetLoadFilename(userConfig.ProjectConfigFilename);
            }
            else
            {
                if(!Directory.Exists(ProjectConfigDirectory))
                    Directory.CreateDirectory(ProjectConfigDirectory);

                if (Directory.GetFiles(ProjectConfigDirectory, "*.yaml").Count() == 0)
                {
                    using (Process proc = new Process())
                    {
                        proc.StartInfo = new ProcessStartInfo
                        {
                            FileName = "BocchiTracker.Client.Config.exe",
                            UseShellExecute = false,
                            Arguments = $"/r"
                        };
                        proc.Start();
                        proc.WaitForExit();
                    }
                }

                var dialogService = Container.Resolve<IDialogService>();
                dialogService.ShowDialog("ConfigFilePickerDialog", new DialogParameters(), r =>
                {
                    var filename = r.Parameters.GetValue<string>("Config");
                    projectConfigRepo.SetLoadFilename(filename);
                });
            }
            
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TicketBasicRegion", typeof(TicketBasicView));
            regionManager.RegisterViewWithRegion("TicketDetailsRegion", typeof(TicketDetailsView));
            regionManager.RegisterViewWithRegion("UtilityRegion", typeof(UtilityView));
            regionManager.RegisterViewWithRegion("UploadFilesRegion", typeof(UploadFilesView));
            
            var projectConfig               = LoadProjectConfig(Container);

            Debug.Assert(projectConfig != null, "Requried ProjectConfig!");
            if (projectConfig == null)
            {
                MessageBox.Show("not fouund ProjectConfig, Crtical Error.");
                Current.Shutdown();
            }

            if(!Directory.Exists(projectConfig.FileSaveDirectory))
                Directory.CreateDirectory(projectConfig.FileSaveDirectory);

            var cacheProvider               = Container.Resolve<ICacheProvider>();
            var connection                  = Container.Resolve<Connection>();
            var dataRepository              = Container.Resolve<IDataRepository>();
            var issueInfoBundle             = Container.Resolve<IssueInfoBundle>();
            var serviceClientFactory        = Container.Resolve<IServiceClientFactory>();
            var authConfigRepositoryFactory = Container.Resolve<IAuthConfigRepositoryFactory>();
            authConfigRepositoryFactory.Initialize(Path.Combine("Configs", nameof(AuthConfig) + "s"));

            _ = connection.StartAsync(projectConfig.Port);

            cacheProvider.SetCacheDirectory(string.IsNullOrEmpty(projectConfig.CacheDirectory) ? Path.GetTempPath() : projectConfig.CacheDirectory);

            Task.Run(async () =>
            {
                var serviceAuthenticator = new ServiceAuthenticator(serviceClientFactory, authConfigRepositoryFactory);

                eventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Authentication started" });
                await Task.Run(() => serviceAuthenticator.ReauthenticateServices(projectConfig.ServiceConfigs));
                
                eventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Getting service infomation" });
                await issueInfoBundle.Initialize(dataRepository, eventAggregator);

                eventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Config applied" });
                eventAggregator.GetEvent<ConfigReloadEvent>().Publish(new ConfigReloadEventParameter(projectConfig, userConfig));

                eventAggregator.GetEvent<ProgressingEvent>().Publish(new ProgressEventParameter { Message = "Initialize: Populate UI" });
                eventAggregator.GetEvent<PopulateUIEvent>().Publish();

                eventAggregator.GetEvent<EndProgressEvent>().Publish();
            });
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(TicketProperty));
            containerRegistry.Register<IFileSystem, FileSystem>();
            containerRegistry.RegisterDialog<ConfigFilePickerDialog, ConfigFilePickerViewModel>();
            containerRegistry.RegisterDialog<UserConfigDialog, UserConfigViewModel>();
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
