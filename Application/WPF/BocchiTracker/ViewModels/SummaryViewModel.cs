using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Ioc;
using BocchiTracker.Event;
using Prism.Events;
using BocchiTracker.IssueInfoCollector;
using Reactive.Bindings;
using YamlDotNet.Core.Tokens;
using Unity.Injection;

namespace BocchiTracker.ViewModels
{
    public class SummaryViewModel : BindableBase
    {
        public ReactiveProperty<string>     Summary { get; }
        public ReactiveProperty<string>     SelectedTicketType { get; }
        public ReactiveCollection<string>   TicketTypes { get; }

        public SummaryViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            Summary = new ReactiveProperty<string>(inIssueInfoBundle.TicketData.Summary);
            Summary.Subscribe(value => inIssueInfoBundle.TicketData.Summary = value);

            SelectedTicketType = new ReactiveProperty<string>();
            SelectedTicketType.Subscribe(value => inIssueInfoBundle.TicketData.TicketType = value);

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
