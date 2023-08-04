using BocchiTracker.Client.Share.Commands;
using BocchiTracker.Client.Share.Events;
using BocchiTracker.Client.Share.Modules;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BocchiTracker.Client.Config.ViewModels
{
    public class ServiceAuthenticationInfo
    {
        public ServiceDefinitions Service { get; set; }

        public ReactiveProperty<Brush> Result { get; set; }

        public ReactiveProperty<string> URL { get; set; }

        public ReactiveProperty<string> ProxyURL { get; set; }

        public ReactiveProperty<AuthConfig> AuthConfig { get; set; }

        public void Initialize(ProjectConfig inProjectConfig, Dictionary<ServiceDefinitions, AuthConfig> inAuthConfigMap)
        {
            Result          = new ReactiveProperty<Brush>(Brushes.Gray);

            URL             = new ReactiveProperty<string>("");
            URL.Subscribe(x => { inProjectConfig.GetServiceConfig(Service).URL = x; });

            ProxyURL        = new ReactiveProperty<string>("");
            ProxyURL.Subscribe(x => { inProjectConfig.GetServiceConfig(Service).ProxyURL = x; });

            AuthConfig = new ReactiveProperty<AuthConfig>(new AuthConfig { APIKey = string.Empty, Username = string.Empty, Password = string.Empty });
            AuthConfig.Subscribe(x => 
            {
                if (inAuthConfigMap.ContainsKey(Service))
                {
                    inAuthConfigMap[Service] = x; return;
                }
                inAuthConfigMap.Add(Service, x); 
            });
        }
    }

    class ServiceViewModel : BindableBase
    {
        public ICommand CheckAuthenticationCommand { get; private set; }

        public ServiceAuthenticationInfo Redmine { get; set; }
        public ServiceAuthenticationInfo Github { get; set; }
        public ServiceAuthenticationInfo Slack { get; set; }

        public ServiceViewModel(IEventAggregator inEventAggregator, ProjectConfig inProjectConfig, Dictionary<ServiceDefinitions, AuthConfig> inAuthConfigMap)
        {
            CheckAuthenticationCommand  = new AsyncCommand(OnCheckAuthenticationCommand);

            Redmine = new ServiceAuthenticationInfo() { Service = ServiceDefinitions.Redmine    };
            Redmine.Initialize(inProjectConfig, inAuthConfigMap);

            Github  = new ServiceAuthenticationInfo() { Service = ServiceDefinitions.Github     };
            Github.Initialize(inProjectConfig, inAuthConfigMap);

            Slack = new ServiceAuthenticationInfo() { Service = ServiceDefinitions.Slack      };
            Slack.Initialize(inProjectConfig, inAuthConfigMap);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
            
            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if(inParam.ProjectConfig != null)
            {
                var serviceConfig = inParam.ProjectConfig.GetServiceConfig(Redmine.Service);
                if(serviceConfig != null)
                {
                    Redmine.URL.Value = serviceConfig.URL;
                    Redmine.ProxyURL.Value = serviceConfig.ProxyURL;
                    if (inParam.AuthConfigs.ContainsKey(Redmine.Service))
                    {
                        if (inParam.AuthConfigs[Redmine.Service] != null)
                            Redmine.AuthConfig.Value = inParam.AuthConfigs[Redmine.Service];
                    }
                }

                serviceConfig = inParam.ProjectConfig.GetServiceConfig(Github.Service);
                if (serviceConfig != null)
                {
                    Github.URL.Value = serviceConfig.URL;
                    Github.ProxyURL.Value = serviceConfig.ProxyURL;
                    if (inParam.AuthConfigs.ContainsKey(Redmine.Service))
                    {
                        if (inParam.AuthConfigs[Github.Service] != null)
                            Github.AuthConfig.Value = inParam.AuthConfigs[Github.Service];
                    }
                }

                serviceConfig = inParam.ProjectConfig.GetServiceConfig(Slack.Service);
                if (serviceConfig != null)
                {
                    Slack.URL.Value = serviceConfig.URL;
                    Slack.ProxyURL.Value = serviceConfig.ProxyURL;
                    if (inParam.AuthConfigs.ContainsKey(Redmine.Service))
                    {
                        if (inParam.AuthConfigs[Slack.Service] != null)
                            Slack.AuthConfig.Value = inParam.AuthConfigs[Slack.Service];
                    }
                }

                Task.Run(OnCheckAuthenticationCommand);
            }
        }

        private async Task OnCheckAuthenticationCommand()
        {
            var serviceClientFactory        
                = (Application.Current as PrismApplication).Container.Resolve<IServiceClientFactory>();

            foreach(var serviceInfo in new List<ServiceAuthenticationInfo> { Redmine, Github, Slack })
            {
                var client = serviceClientFactory.CreateService(serviceInfo.Service);
                if (client == null)
                    continue;

                bool result = await client.Authenticate(serviceInfo.AuthConfig.Value, serviceInfo.URL.Value, serviceInfo.ProxyURL.Value);
                serviceInfo.Result.Value = result
                    ? Brushes.LawnGreen
                    : Brushes.Tomato;
            }
        }

        private void OnSaveConfig()
        {
            var authConfigRepositoryFactory = (Application.Current as PrismApplication).Container.Resolve<IAuthConfigRepositoryFactory>();
            var projectConfigrepository     = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig               = projectConfigrepository.Load();

            foreach(var serviceConfig in projectConfig.ServiceConfigs)
            {
                switch (serviceConfig.Service)
                {
                    case ServiceDefinitions.Redmine:
                        {
                            authConfigRepositoryFactory.Save(serviceConfig.Service, Redmine.AuthConfig.Value);
                            serviceConfig.URL = Redmine.URL.Value;
                            serviceConfig.ProxyURL = Redmine.ProxyURL.Value;
                        }
                        break;
                    case ServiceDefinitions.Github:
                        {
                            authConfigRepositoryFactory.Save(serviceConfig.Service, Github.AuthConfig.Value);
                            serviceConfig.URL = Github.URL.Value;
                            serviceConfig.ProxyURL = Github.ProxyURL.Value;
                        }
                        break;
                    case ServiceDefinitions.Slack:
                        {
                            authConfigRepositoryFactory.Save(serviceConfig.Service, Slack.AuthConfig.Value);
                            serviceConfig.URL = Slack.URL.Value;
                            serviceConfig.ProxyURL = Slack.ProxyURL.Value;
                        }
                        break;
                }
            }
        }
    }
}
