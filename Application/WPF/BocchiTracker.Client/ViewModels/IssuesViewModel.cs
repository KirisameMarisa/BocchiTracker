using BocchiTracker.Config.Configs;
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
using System.Reactive.Linq;
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

        public ReactiveCollection<ServiceDefinitions> ServiceDefinitions { get; set; } = new ReactiveCollection<ServiceDefinitions>();
        public ReactiveProperty<ServiceDefinitions> SelectedService { get; set; } = new ReactiveProperty<ServiceDefinitions>();

        public ReactiveCollection<TicketData> Issues { get; set; } = new ReactiveCollection<TicketData>();
        public IEnumerable SelectedIssues { get; set; } = new ObservableCollection<TicketData>();

        public ReactiveProperty<string> SearchText { get; set; } = new ReactiveProperty<string>();

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

            SelectedService.Subscribe(async (service) => await OnPopulateIssueList(service));
            ServiceDefinitions.CollectionChanged += (_, __) => 
            {
                if (ServiceDefinitions.Count == 1)
                    SelectedService.Value = ServiceDefinitions[0];
            };

            SearchText.Subscribe(async (text) => await OnApplyFilterIssues(text));
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.ServiceConfigs)
            {
                if (string.IsNullOrEmpty(item.URL))
                {
                    continue;
                }

                Task.Run(async () =>
                {
                    var issues = await _getIssues.Get(item.Service, _issueBundle.CustomFieldsListService as CustomFieldListService);
                    if (issues.Count > 0)
                        ServiceDefinitions.AddOnScheduler(item.Service);
                });
            }
        }

        private void OnJumpPlayerLocation()
        {
            foreach (var selectTicketData in SelectedIssues)
            {
                string x, y, z;
                var ticket = selectTicketData as TicketData;
                
                if (ticket.CustomFields.TryGetValue("PlayerPosition.x", out List<string> outX))
                {
                    x = outX.Count > 0 ? outX[0] : null;
                }

                if (ticket.CustomFields.TryGetValue("PlayerPosition.y", out List<string> outY))
                {
                    y = outY.Count > 0 ? outY[0] : null;
                }

                if (ticket.CustomFields.TryGetValue("PlayerPosition.z", out List<string> outZ))
                {
                    z = outZ.Count > 0 ? outZ[0] : null;
                }

                return;
            }
        }

        private bool CanJumpPlayerLocation()
        {
            foreach (var selectTicketData in SelectedIssues)
            {
                var ticket = selectTicketData as TicketData;
                if (ticket.CustomFields.ContainsKey("PlayerPosition.x"))
                    return true;
                if (ticket.CustomFields.ContainsKey("PlayerPosition.y"))
                    return true;
                if (ticket.CustomFields.ContainsKey("PlayerPosition.z"))
                    return true;
                return false;
            }
            return false;
        }

        private void OnOpenInBrowser()
        {
            foreach(var selectTicketData in SelectedIssues) 
            {
                var ticket = selectTicketData as TicketData;
                if (ticket == null)
                    continue;
                OnHyperLink(ticket.Id);
            }
        }

        private void OnHyperLink(string inURL)
        {
            _issueOpener.Open(SelectedService.Value, inURL);
        }

        private async Task OnPopulateIssueList(ServiceDefinitions inService)
        {
            this.Issues.Clear();
            var issues = await this._getIssues.Get(inService, _issueBundle.CustomFieldsListService as CustomFieldListService);
            foreach (var issue in issues)
                Issues.AddOnScheduler(issue);
        }

        private async Task OnApplyFilterIssues(string inText)
        {
            this.Issues.Clear();
            var issues = await this._getIssues.Get(SelectedService.Value, _issueBundle.CustomFieldsListService as CustomFieldListService);
            foreach (var issue in issues)
            {
                if (issue.Summary.Contains(inText) || 
                    issue.Id.Contains(inText) || 
                    (!string.IsNullOrEmpty(issue.Assign.Name) && issue.Assign.Name.Contains(inText)))
                    Issues.AddOnScheduler(issue);
            }
        }
    }
}
