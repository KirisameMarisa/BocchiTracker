using BocchiTracker.Client.Share.Commands;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
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
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;
using System.ComponentModel.DataAnnotations;

namespace BocchiTracker.Client.Config.ViewModels
{
    public class AuthenticationURL
    {
        public ServiceDefinitions Service { get; set; }

        public ReactiveProperty<string> URL { get; set; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ProxyURL { get; set; } = new ReactiveProperty<string>();
    }

    class GeneralViewModel : BindableBase
    {
        public Dictionary<ServiceDefinitions, AuthenticationURL> Authentications = new Dictionary<ServiceDefinitions, AuthenticationURL>
        {
            { ServiceDefinitions.Redmine,   new AuthenticationURL() { Service = ServiceDefinitions.Redmine } },
            { ServiceDefinitions.Github,    new AuthenticationURL() { Service = ServiceDefinitions.Github } },
            { ServiceDefinitions.Slack,     new AuthenticationURL() { Service = ServiceDefinitions.Slack } },
        };

        public AuthenticationURL Redmine
        {
            get => Authentications[ServiceDefinitions.Redmine];
            set => Authentications[ServiceDefinitions.Redmine] = value;
        }

        public AuthenticationURL Github
        {
            get => Authentications[ServiceDefinitions.Github];
            set => Authentications[ServiceDefinitions.Github] = value;
        }

        public AuthenticationURL Slack
        {
            get => Authentications[ServiceDefinitions.Slack];
            set => Authentications[ServiceDefinitions.Slack] = value;
        }

        [Range(1024, 65535, ErrorMessage = "Please enter value in 1024~65535")]
        public ReactiveProperty<string> TcpPort { get; set; }

        public GeneralViewModel(IEventAggregator inEventAggregator, ProjectConfig inProjectConfig)
        {
            TcpPort = new ReactiveProperty<string>("8888").SetValidateAttribute(() => this.TcpPort);
            TcpPort.Subscribe(value => inProjectConfig.Port = int.Parse(value));

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if (inParam.ProjectConfig != null)
            {
                TcpPort.Value = inParam.ProjectConfig.Port.ToString();

                foreach (var serviceConfig in inParam.ProjectConfig.ServiceConfigs)
                {
                    Authentications[serviceConfig.Service].URL.Value = serviceConfig.URL;
                    Authentications[serviceConfig.Service].ProxyURL.Value = serviceConfig.ProxyURL;
                }
            }
        }

        private void OnSaveConfig()
        {
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigrepository.Load();

            projectConfig.Port = int.Parse(TcpPort.Value);

            foreach (var (service, item) in Authentications)
            {
                var serviceConfig = projectConfig.GetServiceConfig(service);
                if(serviceConfig != null)
                {
                    serviceConfig.URL = item.URL.Value;
                    serviceConfig.ProxyURL = item.ProxyURL.Value;
                }
            }
        }
    }
}
