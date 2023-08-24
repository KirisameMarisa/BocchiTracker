using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using BocchiTracker.Client.Share.Commands;
using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using Prism.Ioc;
using YamlDotNet.Core.Tokens;

namespace BocchiTracker.Client.ViewModels.UserConfigParts
{
    public class AuthenticationInfo
    {
        public string URL { get; set; }

        public string ProxyURL { get; set; }

        public ServiceDefinitions Service { get; set; }

        public ReactiveProperty<bool> IsEnable { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<Brush> Result { get; set; } = new ReactiveProperty<Brush>(Brushes.Gray);

        public ReactiveProperty<AuthConfig> AuthConfig { get; set; }
    }

    public class AuthenticationParts : BindableBase, IConfig
    {
        public ICommand CheckAuthenticationCommand { get; private set; }

        public Dictionary<ServiceDefinitions, AuthenticationInfo> Authentications = new Dictionary<ServiceDefinitions, AuthenticationInfo>
        {
            { ServiceDefinitions.Redmine,   new AuthenticationInfo() },
            { ServiceDefinitions.Github,    new AuthenticationInfo() },
            { ServiceDefinitions.Slack,     new AuthenticationInfo() },
        };

        public AuthenticationInfo Redmine 
        { 
            get => Authentications[ServiceDefinitions.Redmine];
            set => Authentications[ServiceDefinitions.Redmine] = value;
        }

        public AuthenticationInfo Github 
        {
            get => Authentications[ServiceDefinitions.Github];
            set => Authentications[ServiceDefinitions.Github] = value;
        }

        public AuthenticationInfo Slack 
        {
            get => Authentications[ServiceDefinitions.Slack];
            set => Authentications[ServiceDefinitions.Slack] = value;
        }

        private IAuthConfigRepositoryFactory _authConfigRepositoryFactory;

        public AuthenticationParts(IAuthConfigRepositoryFactory inAuthConfigRepositoryFactory, CachedConfigRepository<ProjectConfig> inProjectConfigRepository)
        {
            _authConfigRepositoryFactory = inAuthConfigRepositoryFactory;
            CheckAuthenticationCommand = new AsyncCommand(OnCheckAuthenticationCommand);

            foreach(var serviceConfig in inProjectConfigRepository.Load().ServiceConfigs)
            {
                Authentications[serviceConfig.Service].AuthConfig   = new ReactiveProperty<AuthConfig>(_authConfigRepositoryFactory.Load(serviceConfig.Service));
                if (Authentications[serviceConfig.Service].AuthConfig.Value == null)
                    Authentications[serviceConfig.Service].AuthConfig.Value = new AuthConfig();

                Authentications[serviceConfig.Service].URL          = serviceConfig.URL;
                Authentications[serviceConfig.Service].ProxyURL     = serviceConfig.ProxyURL;
                Authentications[serviceConfig.Service].Service      = serviceConfig.Service;

                if(!string.IsNullOrEmpty(Authentications[serviceConfig.Service].URL))
                    Authentications[serviceConfig.Service].IsEnable.Value = true;
            }
            Task.Run(OnCheckAuthenticationCommand);
        }

        public void Save()
        {
            foreach(var (service, authenticationInfo) in Authentications)
            {
                if (!authenticationInfo.IsEnable.Value)
                    continue;

                _authConfigRepositoryFactory.Save(service, authenticationInfo.AuthConfig.Value);
            }
        }

        private async Task OnCheckAuthenticationCommand()
        {
            var serviceClientFactory
                = (Application.Current as PrismApplication).Container.Resolve<IServiceClientFactory>();

            foreach (var serviceInfo in new List<AuthenticationInfo> { Redmine, Github, Slack })
            {
                if (!serviceInfo.IsEnable.Value)
                    continue;

                var client = serviceClientFactory.CreateService(serviceInfo.Service);
                if (client == null)
                    continue;

                bool result = await client.Authenticate(serviceInfo.AuthConfig.Value, serviceInfo.URL, serviceInfo.ProxyURL);
                serviceInfo.Result.Value = result
                    ? Brushes.LawnGreen
                    : Brushes.Tomato;
            }
        }
    }
}
