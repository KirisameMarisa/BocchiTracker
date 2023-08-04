using BocchiTracker.Client.Config.Controls;
using BocchiTracker.Client.Share.Events;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BocchiTracker.Client.Config.ViewModels
{
    public class ValueMapBase : BindableBase
    {
        public ICommand AddItemCommand { get; private set; }
        public ICommand RemoveItemCommand { get; private set; }
        public ReactiveCollection<ServiceValueMapping> ValueMappings { get; set; } = new ReactiveCollection<ServiceValueMapping>();

        public ValueMapBase()
        {
            AddItemCommand = new DelegateCommand<string>(OnAddItem);
            RemoveItemCommand = new DelegateCommand<string>(OnRemoveItem);
        }

        public void OnAddItem(string inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return;
            if (Find(inValue) != null)
                return;
            ValueMappings.Add(new ServiceValueMapping(inValue));
        }

        public void OnRemoveItem(string inValue)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var itemToRemove = Find(inValue);
                if (itemToRemove != null)
                    ValueMappings.Remove(itemToRemove);
            });
        }

        public ServiceValueMapping Find(string inValue)
        {
            return ValueMappings.FirstOrDefault(kvp => kvp.Definition.Value == inValue);
        }
    }

    class TicketViewModel : BindableBase
    {
        public ValueMapBase TicketTypes { get; set; } = new ValueMapBase();
        public ValueMapBase Priorities  { get; set; } = new ValueMapBase();
        public ValueMapBase IssueGrades { get; set; } = new ValueMapBase();
        public ValueMapBase QueryFields { get; set; } = new ValueMapBase();

        public TicketViewModel(IEventAggregator inEventAggregator)
        {
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
                ApplyProjectConfigToValueMappings(
                    TicketTypes,
                    inParam.ProjectConfig.TicketTypes,
                    inParam.ProjectConfig.ServiceConfigs == null ? null : inParam.ProjectConfig.ServiceConfigs.Where(x => x.TicketTypeMappings != null).Select(x => (x.Service, x.TicketTypeMappings)).ToList());

                ApplyProjectConfigToValueMappings(
                    Priorities, 
                    inParam.ProjectConfig.Priorities,
                    inParam.ProjectConfig.ServiceConfigs == null ? null : inParam.ProjectConfig.ServiceConfigs.Where(x => x.PriorityMappings != null).Select(x => (x.Service, x.PriorityMappings)).ToList());

                ApplyProjectConfigToValueMappings(
                    IssueGrades,
                    inParam.ProjectConfig.IssueGrades,
                    inParam.ProjectConfig.ServiceConfigs == null ? null : inParam.ProjectConfig.ServiceConfigs.Where(x => x.IssueGradeMappings != null).Select(x => (x.Service, x.IssueGradeMappings)).ToList());

                ApplyProjectConfigToValueMappings(
                    QueryFields,
                    inParam.ProjectConfig.QueryFields,
                    inParam.ProjectConfig.ServiceConfigs == null ? null : inParam.ProjectConfig.ServiceConfigs.Where(x => x.QueryFieldMappings != null).Select(x => (x.Service, x.QueryFieldMappings)).ToList());
            }
        }

        private void ApplyProjectConfigToValueMappings(ValueMapBase ioValueMappingsContainer, List<string> inDefinitions, List<(ServiceDefinitions, List<ValueMapping>)> inServiceValueMappings)
        {
            foreach (var definition in inDefinitions)
                ioValueMappingsContainer.ValueMappings.Add(new ServiceValueMapping(definition));

            if (inServiceValueMappings == null)
                return;

            foreach (var (service, values) in inServiceValueMappings)
            {
                foreach (var mapping in values)
                {
                    ServiceValueMapping valueMapping = ioValueMappingsContainer.ValueMappings.ToList().Find(vm => vm.Definition.Value == mapping.Definition);
                    if (valueMapping == null)
                        continue;

                    valueMapping.SetServiceName(service, mapping.Name);
                }
            }
        }

        private void OnSaveConfig()
        {
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig           = projectConfigrepository.Load();

            projectConfig.TicketTypes = TicketTypes.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.TicketTypeMappings.Clear();
                foreach (var valueMapping in TicketTypes.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.TicketTypeMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.Priorities = Priorities.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.PriorityMappings.Clear();
                foreach (var valueMapping in Priorities.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.PriorityMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.IssueGrades = IssueGrades.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.IssueGradeMappings.Clear();
                foreach (var valueMapping in IssueGrades.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.IssueGradeMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }

            projectConfig.QueryFields = QueryFields.ValueMappings.Select(x => x.Definition.Value).ToList();
            foreach (var serviceConfig in projectConfig.ServiceConfigs)
            {
                serviceConfig.QueryFieldMappings.Clear();
                foreach (var valueMapping in QueryFields.ValueMappings)
                {
                    var service = serviceConfig.Service;
                    serviceConfig.QueryFieldMappings.Add(new ValueMapping { Definition = valueMapping.Definition.Value, Name = valueMapping.GetServiceName(service) });
                }
            }
        }
    }
}
