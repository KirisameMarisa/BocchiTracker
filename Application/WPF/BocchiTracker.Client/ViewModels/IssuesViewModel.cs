using BocchiTracker.ApplicationInfoCollector;
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
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
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
        private IGetIssues _getIssues;
        private IIssueOpener _issueOpener;

        public ConnectTo ConnectTo { get; set; }

        private ProjectConfig _projectConfig;

        public IssuesViewModel(
            IEventAggregator inEventAggregator, 
            IGetIssues inGetIssues, IIssueOpener inIssueOpener, 
            TicketProperty inTicketProperty)
        {
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            _getIssues = inGetIssues;
            _issueOpener = inIssueOpener;

            HyperlinkCommand = new DelegateCommand<string>(OnHyperLink);     
            JumpPlayerLocationCommand = new DelegateCommand(OnJumpPlayerLocation);
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
            _projectConfig = inParam.ProjectConfig;

            foreach (var item in _projectConfig.ServiceConfigs)
            {
                if (string.IsNullOrEmpty(item.URL))
                {
                    continue;
                }

                Task.Run(async () =>
                {
                    var issues = await _getIssues.Get(item);
                    if (issues.Count > 0)
                        ServiceDefinitions.AddOnScheduler(item.Service);
                });
            }
        }

        private void OnJumpPlayerLocation()
        {
            var connectApplication = ConnectTo.Selected.Value as AppStatusBundle;
            if (connectApplication == null)
                return;

            var selectedTickets = SelectedIssues.Cast<TicketData>();
            var ticket = selectedTickets.FirstOrDefault() ?? null;
            if (ticket == null)
                return;
            
            float x = float.NaN, y = float.NaN, z = float.NaN;
            if (ticket.CustomFields.TryGetValue("PlayerPosition.x", out List<string> outX))
            {
                if(outX.Count > 0 && !string.IsNullOrEmpty(outX[0]) && float.TryParse(outX[0], out float x_))
                {
                    x = x_;
                }
            }

            if (ticket.CustomFields.TryGetValue("PlayerPosition.y", out List<string> outY))
            {
                if (outY.Count > 0 && !string.IsNullOrEmpty(outY[0]) && float.TryParse(outY[0], out float y_))
                {
                    y = y_;
                }
            }

            if (ticket.CustomFields.TryGetValue("PlayerPosition.z", out List<string> outZ))
            {
                if (outZ.Count > 0 && !string.IsNullOrEmpty(outZ[0]) && float.TryParse(outZ[0], out float z_))
                {
                    z = z_;
                }
            }

            string stage = string.Empty;
            if (ticket.CustomFields.TryGetValue("PlayerPosition.stage", out List<string> outstage))
            {
                stage = outstage.Count > 0 && !string.IsNullOrEmpty(outstage[0]) ? outstage[0] : null;
            }

            _eventAggregator.GetEvent<RequestQueryEvent>().Publish(new JumpRequestEventParameter(connectApplication.AppBasicInfo.ClientID, x, y, z, stage));
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
            if (_projectConfig == null)
                return;

            this.Issues.Clear();
            var issues = await this._getIssues.Get(_projectConfig.GetServiceConfig(inService));
            foreach (var issue in issues)
                Issues.AddOnScheduler(issue);
        }

        private async Task OnApplyFilterIssues(string inText)
        {
            if (_projectConfig == null)
                return;

            this.Issues.Clear();
            var issues = await this._getIssues.Get(_projectConfig.GetServiceConfig(SelectedService.Value));
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
