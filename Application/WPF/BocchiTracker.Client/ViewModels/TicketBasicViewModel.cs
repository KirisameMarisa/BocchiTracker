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

namespace BocchiTracker.Client.ViewModels
{
    public class TicketBasicViewModel : BindableBase
    {
        public TicketProperty TicketProperty { get; set; }
        public ReactiveCollection<string> TicketTypes { get; }
        public ICommand RunConfigCommand { get; }

        private string useProjectConfig = default!;

        private IDialogService _dialogService;

        public TicketBasicViewModel(IEventAggregator inEventAggregator, IDialogService inDialogService, TicketProperty inTicketProperty)
        {
            TicketProperty = inTicketProperty;
            TicketTypes = new ReactiveCollection<string>();
            RunConfigCommand = new DelegateCommand(OnRunConfig);
            _dialogService = inDialogService;

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

            if(inParam.UserConfig != null)
            {
                useProjectConfig = inParam.UserConfig.ProjectConfigFilename;
            }
        }

        private void OnIssueSubmittedEvent(IssueSubmittedEventParameter inParam)
        {
            TicketProperty.Summary.Value = string.Empty;
            TicketProperty.Description.Value = string.Empty;
        }

        private void OnRunConfig()
        {
            _dialogService.ShowDialog("UserConfigDialog", new DialogParameters(), r =>
            {

            });
        }
    }
}
