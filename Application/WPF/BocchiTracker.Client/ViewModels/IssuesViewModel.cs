using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.Data;
using BocchiTracker.ModelEvent;
using Prism.Events;
using Prism.Mvvm;

namespace BocchiTracker.Client.ViewModels
{
    public class IssuesViewModel : BindableBase
    {
        public IssueListParts.ListPart IssueList { get; set; }
        public IssueListParts.SearchPart IssueSearch { get; set; }

        private IEventAggregator _eventAggregator;

        public IssuesViewModel(IEventAggregator inEventAggregator, IGetIssues inGetIssues, IIssueOpener inIssueOpener, TicketProperty inTicketProperty)
        {
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            IssueList = new IssueListParts.ListPart(inGetIssues, inEventAggregator, inIssueOpener);
            IssueSearch = new IssueListParts.SearchPart(inTicketProperty, IssueList);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.ServiceConfigs)
            {
                if (string.IsNullOrEmpty(item.URL))
                    continue;

                IssueList.ServiceDefinitions.Add(item.Service, item);

                var issues = IssueList.GetAllIssues(item.Service);
                if (issues.Count > 0)
                    IssueSearch.ServiceDefinitions.AddOnScheduler(item.Service);
            }

            IssueList.CurrentService = () => IssueSearch.SelectedService.Value;
            IssueList.CurrentConnectionStatus = () => IssueSearch.ConnectTo.Selected.Value as AppStatusBundle;
        }
    }
}
