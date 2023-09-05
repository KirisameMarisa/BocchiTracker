using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Commands;

namespace BocchiTracker.Client.Config.ViewModels
{
    /// <summary>
    /// View model for managing description formats in the configuration.
    /// </summary>
    public class DescriptionFormatViewModel : BindableBase
    {
        // Reactive properties to hold and bind data
        public ReactiveCollection<string> BuiltinFormats { get; set; } = new ReactiveCollection<string>();
        public ICommand SelectedBuiltinFormatButton { get; set; }

        public ReactiveCollection<ServiceDefinitions> ServiceDefinitions { get; set; } = new ReactiveCollection<ServiceDefinitions>();
        public ReactiveProperty<ServiceDefinitions> SelectedService { get; set; } = new ReactiveProperty<ServiceDefinitions>();
        public ReactiveProperty<string> DescriptionFormat { get; set; } = new ReactiveProperty<string>();


        // Mapping of service definitions to description formats
        private Dictionary<ServiceDefinitions, string> _descriptionFormatMap = new Dictionary<ServiceDefinitions, string>();

        /// <summary>
        /// Constructor for the DescriptionFormatViewModel.
        /// </summary>
        /// <param name="inEventAggregator">The event aggregator used for communication between components.</param>
        public DescriptionFormatViewModel(IEventAggregator inEventAggregator)
        {
            // Subscribe to events for configuration reloading and application exit
            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);

            // Add built-in description formats and handle selection changes
            BuiltinFormats.Add(nameof(DescriptionFormatBuiltin.Simple));
            BuiltinFormats.Add(nameof(DescriptionFormatBuiltin.Chat));
            BuiltinFormats.Add(nameof(DescriptionFormatBuiltin.Detail));
            SelectedBuiltinFormatButton = new DelegateCommand<string>((x) => 
            {
                if (x == nameof(DescriptionFormatBuiltin.Simple))
                    DescriptionFormat.Value = DescriptionFormatBuiltin.Simple;
                if (x == nameof(DescriptionFormatBuiltin.Chat))
                    DescriptionFormat.Value = DescriptionFormatBuiltin.Chat;
                if (x == nameof(DescriptionFormatBuiltin.Detail))
                    DescriptionFormat.Value = DescriptionFormatBuiltin.Detail;
            });

            // Handle selection changes of the service and associated description format
            SelectedService.Subscribe(x =>
            {
                if (_descriptionFormatMap.ContainsKey(x))
                    DescriptionFormat.Value = _descriptionFormatMap[x];
            });

            // Handle changes in the description format and update the mapping
            DescriptionFormat.Subscribe(x =>
            {
                if (_descriptionFormatMap.ContainsKey(SelectedService.Value))
                    _descriptionFormatMap[SelectedService.Value] = DescriptionFormat.Value;
            });
        }

        // Handle configuration reload event
        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            // Populate service definitions and description format mapping
            foreach (var item in inParam.ProjectConfig.ServiceConfigs)
            {
                _descriptionFormatMap.Add(item.Service, string.Empty);
                switch(item.Service)
                {
                    case ServiceClientData.ServiceDefinitions.Slack:
                    case ServiceClientData.ServiceDefinitions.Discord:
                        _descriptionFormatMap[item.Service] = DescriptionFormat.Value = DescriptionFormatBuiltin.Chat; break;

                    case ServiceClientData.ServiceDefinitions.Github:
                    case ServiceClientData.ServiceDefinitions.Glitlab:
                        _descriptionFormatMap[item.Service] = DescriptionFormat.Value = DescriptionFormatBuiltin.Detail; break;

                    case ServiceClientData.ServiceDefinitions.JIRA:
                    case ServiceClientData.ServiceDefinitions.Redmine:
                        _descriptionFormatMap[item.Service] = DescriptionFormat.Value = DescriptionFormatBuiltin.Simple; break;
                }
                ServiceDefinitions.Add(item.Service);
            }

            foreach (var service in ServiceDefinitions)
            {
                var serviceConfig = inParam.ProjectConfig.GetServiceConfig(service);
                if (serviceConfig == null || string.IsNullOrEmpty(serviceConfig.DescriptionFormat))
                {
                    continue;
                }

                if(!string.IsNullOrEmpty(serviceConfig.DescriptionFormat))
                    _descriptionFormatMap[serviceConfig.Service] = serviceConfig.DescriptionFormat;
            }

            SelectedService.Value = ServiceClientData.ServiceDefinitions.Redmine;
        }

        // Handle save configuration on application exit
        private void OnSaveConfig()
        {
            // Retrieve and update project configuration
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigrepository.Load();

            foreach (var (service, descFormat) in _descriptionFormatMap)
            {
                var serviceConfig = projectConfig.GetServiceConfig(service);
                if (serviceConfig != null)
                    serviceConfig.DescriptionFormat = descFormat;
            }
        }
    }
}