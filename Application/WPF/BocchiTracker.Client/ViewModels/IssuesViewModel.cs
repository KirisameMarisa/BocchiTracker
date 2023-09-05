using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.Data;
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
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BocchiTracker.Client.ViewModels
{
    public class IssueItem
    {
        public ICommand HyperlinkCommand { get; private set; }

        public ICommand JumpPlayerLocationCommand { get; private set; }

        public Func<AppStatusBundle> CurrentConnectionStatus { get; set; }

        public Func<ServiceDefinitions> CurrentService { get; set; }

        public TicketData TicketData { get; set; }

        private readonly IEventAggregator _eventAggregator;

        private readonly IIssueOpener _issueOpener;

        public IssueItem(TicketData inTicketData, IEventAggregator inEventAggregator, IIssueOpener inIssueOpener)
        {
            TicketData = inTicketData;

            HyperlinkCommand = new DelegateCommand(OnOpenInBrowser);
            JumpPlayerLocationCommand = new DelegateCommand(OnJumpPlayerLocation, CanJumpPlayer);

            _eventAggregator = inEventAggregator;
            _issueOpener = inIssueOpener;
        }

        public bool CanJumpPlayer()
        {
            if (TicketData.CustomFields == null)
                return false;

            List<bool> validValues = new List<bool>();
            if (TicketData.CustomFields.TryGetValue("PlayerPosition.x", out List<string> outX))
            {
                if (outX.Count > 0 && !string.IsNullOrEmpty(outX[0]) && float.TryParse(outX[0], out float x_))
                {
                    validValues.Add(true);
                }
            }

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.y", out List<string> outY))
            {
                if (outY.Count > 0 && !string.IsNullOrEmpty(outY[0]) && float.TryParse(outY[0], out float y_))
                {
                    validValues.Add(true);
                }
            }

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.z", out List<string> outZ))
            {
                if (outZ.Count > 0 && !string.IsNullOrEmpty(outZ[0]) && float.TryParse(outZ[0], out float z_))
                {
                    validValues.Add(true);
                }
            }
            return validValues.Count >= 2;
        }

        private void OnJumpPlayerLocation()
        {
            var connectApplication = CurrentConnectionStatus?.Invoke();
            if (connectApplication == null)
                return;

            float x = float.NaN, y = float.NaN, z = float.NaN;
            if (TicketData.CustomFields.TryGetValue("PlayerPosition.x", out List<string> outX))
                x = float.Parse(outX[0]);

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.y", out List<string> outY))
                y = float.Parse(outY[0]);

            if (TicketData.CustomFields.TryGetValue("PlayerPosition.z", out List<string> outZ))
                z = float.Parse(outZ[0]);

            string stage = string.Empty;
            if (TicketData.CustomFields.TryGetValue("PlayerPosition.stage", out List<string> outstage))
                stage = outstage.Count > 0 && !string.IsNullOrEmpty(outstage[0]) ? outstage[0] : null;

            _eventAggregator.GetEvent<RequestQueryEvent>().Publish(new JumpRequestEventParameter(connectApplication.AppBasicInfo.ClientID, x, y, z, stage));
        }

        public void OnOpenInBrowser()
        {
            if (CurrentService == null)
                return;

            ServiceDefinitions service = CurrentService.Invoke();
            _issueOpener.Open(service, TicketData.Id);
        }
    }

    public class IssuesViewModel : BindableBase
    {
        public ICommand OpenInBrowserCommand { get; private set; }

        public ReactiveCollection<ServiceDefinitions> ServiceDefinitions { get; set; } = new ReactiveCollection<ServiceDefinitions>();
        public ReactiveProperty<ServiceDefinitions> SelectedService { get; set; } = new ReactiveProperty<ServiceDefinitions>();

        public ConnectTo ConnectTo { get; set; }

        public ReactiveCollection<IssueItem> Issues { get; set; } = new ReactiveCollection<IssueItem>();
        public IEnumerable SelectedIssues { get; set; } = new ObservableCollection<TicketData>();

        public ReactiveProperty<string> SearchText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> ShowItemsWithLocation { get; set; } = new ReactiveProperty<bool>(false);

        private IEventAggregator _eventAggregator;
        private IGetIssues _getIssues;
        private IIssueOpener _issueOpener;

        private ProjectConfig _projectConfig;

        public IssuesViewModel(IEventAggregator inEventAggregator, IGetIssues inGetIssues, IIssueOpener inIssueOpener, TicketProperty inTicketProperty)
        {
            _getIssues = inGetIssues;
            _issueOpener = inIssueOpener;
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            OpenInBrowserCommand = new DelegateCommand(OnOpenInBrowser);

            ConnectTo = new ConnectTo(inTicketProperty);

            SelectedService.Subscribe(async (service) => await OnPopulateIssueList(service));
            ServiceDefinitions.CollectionChanged += (_, __) =>
            {
                if (ServiceDefinitions.Count == 1)
                    SelectedService.Value = ServiceDefinitions[0];
            };

            SearchText.Subscribe(async (_) => await OnApplyFilterIssues());
            ShowItemsWithLocation.Subscribe(async (_) => await OnApplyFilterIssues());
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

        private void OnOpenInBrowser()
        {
            foreach (var item in SelectedIssues)
            {
                var obj = item as IssueItem;
                if (obj == null)
                    continue;
                obj.OnOpenInBrowser();
            }
        }

        private async Task OnPopulateIssueList(ServiceDefinitions inService)
        {
            if (_projectConfig == null)
                return;

            this.Issues.Clear();
            var issues = await this._getIssues.Get(_projectConfig.GetServiceConfig(inService));
            foreach (var issue in issues)
                Issues.AddOnScheduler(new IssueItem(issue, _eventAggregator, _issueOpener)
                {
                    CurrentService = () => SelectedService.Value,
                    CurrentConnectionStatus = () => ConnectTo.Selected.Value as AppStatusBundle
                });
        }

        private async Task OnApplyFilterIssues()
        {
            if (_projectConfig == null)
                return;

            var filterText = SearchText.Value;
            var showItemsWithLocation = ShowItemsWithLocation.Value;

            this.Issues.Clear();
            var issues = await this._getIssues.Get(_projectConfig.GetServiceConfig(SelectedService.Value));
            foreach (var issue in issues)
            {

                if (string.IsNullOrEmpty(filterText) ||
                    issue.Summary.Contains(filterText) ||
                    issue.Id.Contains(filterText) ||
                    (!string.IsNullOrEmpty(issue.Assign.Name) && issue.Assign.Name.Contains(filterText)))
                {
                    var item = new IssueItem(issue, _eventAggregator, _issueOpener)
                    {
                        CurrentService = () => SelectedService.Value,
                        CurrentConnectionStatus = () => ConnectTo.Selected.Value as AppStatusBundle
                    };

                    if (showItemsWithLocation)
                    {
                        if (item.CanJumpPlayer())
                            Issues.AddOnScheduler(item);
                    }
                    else
                    {
                        Issues.AddOnScheduler(item);
                    }
                }
            }
        }
    }
}
