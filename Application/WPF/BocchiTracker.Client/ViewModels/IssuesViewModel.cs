using BocchiTracker.CrossServiceReporter;
using BocchiTracker.Data;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientData;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BocchiTracker.Client.ViewModels
{
    public class IssuesViewModel : BindableBase
    {
        public ICommand HyperlinkCommand { get; private set; }
        public ICommand JumpPlayerLocationCommand { get; private set; }
        public ICommand OpenInBrowserCommand { get; private set; }

        public ReactiveProperty<ServiceDefinitions> ServiceDefinitionsProperty { get; set; } = new ReactiveProperty<ServiceDefinitions>();
        public ReactiveCollection<TicketData> Issus { get; set; } = new ReactiveCollection<TicketData>();
        public IEnumerable SelectedIssues { get; set; } = new ObservableCollection<TicketData>();

        private IEventAggregator _eventAggregator;
        private IssueInfoBundle _issueBundle;
        private IGetIssues _getIssues;
        private IIssueOpener _issueOpener;

        public ConnectTo ConnectTo { get; set; }

        public IssuesViewModel(
            IEventAggregator inEventAggregator, 
            IGetIssues inGetIssues, IIssueOpener inIssueOpener, 
            IssueInfoBundle inIssueBundle,
            TicketProperty inTicketProperty)
        {
            ServiceDefinitionsProperty.Value = ServiceDefinitions.Github;

            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            _getIssues = inGetIssues;
            _issueOpener = inIssueOpener;
            _issueBundle = inIssueBundle;

            HyperlinkCommand = new DelegateCommand<string>(OnHyperLink);
            JumpPlayerLocationCommand = new DelegateCommand(OnJumpPlayerLocation, CanJumpPlayerLocation);
            OpenInBrowserCommand = new DelegateCommand(OnOpenInBrowser);

            ConnectTo = new ConnectTo(inTicketProperty);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            Task.Run(async () =>
            {
                var issues = await _getIssues.Get(ServiceDefinitionsProperty.Value, _issueBundle.CustomFieldsListService as CustomFieldListService);
                foreach (var issue in issues)
                    Issus.AddOnScheduler(issue);
            });
        }

        private void OnJumpPlayerLocation()
        {

        }

        private bool CanJumpPlayerLocation()
        {
            return false;
        }

        private void OnOpenInBrowser()
        {
            foreach(var dynamicTicketData in SelectedIssues) 
            {
                var ticket = dynamicTicketData as TicketData;
                if (ticket == null)
                    continue;
                OnHyperLink(ticket.Id);
            }
        }

        private void OnHyperLink(string inURL)
        {
            _issueOpener.Open(ServiceDefinitionsProperty.Value, inURL);
        }
    }
}
