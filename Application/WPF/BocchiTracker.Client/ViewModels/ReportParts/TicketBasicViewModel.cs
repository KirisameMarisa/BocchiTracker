using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Data;
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
using System.Windows;
using Slack.NetStandard.Messages.Blocks;
using System.Diagnostics;
using System.IO;
using BocchiTracker.ModelEvent;
using Prism.Services.Dialogs;

namespace BocchiTracker.Client.ViewModels.ReportParts
{
    public class TicketBasicViewModel : BindableBase
    {
        public TicketProperty TicketProperty { get; set; }
        public ReactiveCollection<string> TicketTypes { get; }
        public ICommand RunConfigCommand { get; }

        public TicketBasicViewModel(IEventAggregator inEventAggregator, TicketProperty inTicketProperty)
        {
            TicketProperty = inTicketProperty;
            TicketTypes = new ReactiveCollection<string>();

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
            inEventAggregator
                .GetEvent<IssueSubmittedEvent>()
                .Subscribe(OnIssueSubmittedEvent, ThreadOption.UIThread);
            

        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.TicketTypes)
                TicketTypes.Add(item);
        }

        private void OnIssueSubmittedEvent(IssueSubmittedEventParameter inParam)
        {
            TicketProperty.Summary.Value = string.Empty;
            TicketProperty.Description.Value = string.Empty;
        }
    }
}
