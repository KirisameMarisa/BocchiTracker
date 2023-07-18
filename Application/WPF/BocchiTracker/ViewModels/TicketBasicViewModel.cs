using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.Data;
using BocchiTracker.Event;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Prism.Commands;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public class TicketBasicViewModel : BindableBase
    {
        public TicketProperty TicketProperty { get; set; }
        public ReactiveCollection<string> TicketTypes { get; }

        public TicketBasicViewModel(IEventAggregator inEventAggregator, TicketProperty inTicketProperty)
        {
            TicketProperty = inTicketProperty;
            TicketTypes = new ReactiveCollection<string>();

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.TicketTypes)
                TicketTypes.Add(item);
        }
    }
}
