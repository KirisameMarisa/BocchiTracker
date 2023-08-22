using BocchiTracker.Client.Config.Views;
using BocchiTracker.Client.Share.Controls;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;
using System.IO.Abstractions;
using System.Windows;
using BocchiTracker.ServiceClientAdapters;
using System.Collections.Generic;
using BocchiTracker.ProcessLinkQuery;
using System.Linq;
using System.IO;
using System.Diagnostics;
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Client.Config
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        bool NeedClientRestart = false;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ApplicationExitEvent>().Publish();

            var authConfigRepositoryFactory = (Application.Current as PrismApplication).Container.Resolve<IAuthConfigRepositoryFactory>();
            authConfigRepositoryFactory.Initialize(Path.Combine("Configs", nameof(AuthConfig) + "s"));
            
            var projectConfigRepo = Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigRepo.Load();

            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                var service = serviceConfig.Service;
                var authConfig = authConfigRepositoryFactory.Load(service);
                authConfigRepositoryFactory.Save(service, authConfig);
            }
            projectConfigRepo.Save(projectConfig);

            if(NeedClientRestart)
            {
                var application = "bocchitracker.client";
                var process = Process.GetProcessesByName(application);
                foreach (var i in process) { i.Kill(); }
                Process.Start(application + ".exe");
            }
            base.OnExit(e);
        }

        protected override void OnInitialized()
        {
            string[] cmds = System.Environment.GetCommandLineArgs();
            var configRepo = Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            if (cmds.Length > 1)
            {
                var filename = cmds[1];
                if(File.Exists(filename))
                    configRepo.SetLoadFilename(cmds[1]);
            }

            if(cmds.Contains("/r"))
                NeedClientRestart = true;

            if(string.IsNullOrEmpty(configRepo.GetLoadFilename()) || !File.Exists(configRepo.GetLoadFilename()))
            {
                var dialogService = Container.Resolve<IDialogService>();
                dialogService.ShowDialog("ConfigFilePickerDialog", new DialogParameters($"EnableFileCreation={true}"), r =>
                {
                    var filename = r.Parameters.GetValue<string>("Config");
                    configRepo.SetLoadFilename(filename);
                });
            }

            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ServiceRegion",   typeof(ServiceView));
            regionManager.RegisterViewWithRegion("TicketRegion",    typeof(TicketView));
            regionManager.RegisterViewWithRegion("DirectoryRegion", typeof(DirectoryView));
            regionManager.RegisterViewWithRegion("NetworkRegion",   typeof(NetworkView));

            var eventAggregator = Container.Resolve<IEventAggregator>();
            {
                var authConfigRepositoryFactory = Container.Resolve<IAuthConfigRepositoryFactory>();
                authConfigRepositoryFactory.Initialize(Path.Combine("Configs", nameof(AuthConfig) + "s"));
                var projectConfigRepo           = Container.Resolve<CachedConfigRepository<ProjectConfig>>();
                var userConfigRepo              = Container.Resolve<CachedConfigRepository<UserConfig>>();
                var variableDump                = Container.Resolve<VariableDump>();

                var projectConfig = projectConfigRepo.Load();
                if(projectConfig == null)
                    projectConfigRepo.Save(new ProjectConfig());

                projectConfig = projectConfigRepo.Load();
                projectConfig.QueryFields.Clear();
                var cIgnoreTable = new string[] { "ScreenshotData", "RequestQuery", "Packet" };
                foreach (var (className, variables) in variableDump.ClassAndPropertyNames)
                {
                    string value = className;
                    if (cIgnoreTable.Contains(value))
                        continue;

                    foreach(var variable in variables)
                        projectConfig.QueryFields.Add($"{value}.{variable}");
                }

                eventAggregator
                    .GetEvent<ConfigReloadEvent>()
                    .Publish(new ConfigReloadEventParameter(projectConfigRepo.Load(), userConfigRepo.Load()) 
                    {
                        AuthConfigs = new Dictionary<ServiceDefinitions, AuthConfig> 
                        {
                            { ServiceDefinitions.Redmine,    authConfigRepositoryFactory.Load(ServiceDefinitions.Redmine)    },
                            { ServiceDefinitions.Github,     authConfigRepositoryFactory.Load(ServiceDefinitions.Github)     },
                            { ServiceDefinitions.Slack,      authConfigRepositoryFactory.Load(ServiceDefinitions.Slack)      },
                        }
                    });
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileSystem, FileSystem>();
            containerRegistry.RegisterDialog<ConfigFilePickerDialog, ConfigFilePickerViewModel>();
            containerRegistry.RegisterInstance(new Dictionary<ServiceDefinitions, AuthConfig>());
            containerRegistry.RegisterInstance(new VariableDump("Query.schema.json"));
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ConfigModule>();
            moduleCatalog.AddModule<ServiceClientAdaptersModule>(
                dependsOn: new string[]
                {
                    typeof(ConfigModule).Name
                });
        }
    }
}
