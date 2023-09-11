using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.Data;
using BocchiTracker.ModelEvent;
using Prism.Events;
using Prism.Mvvm;

namespace BocchiTracker.Client.ViewModels.IssueListParts
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
            IssueSearch = new IssueListParts.SearchPart(inEventAggregator, inTicketProperty, IssueList);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            IssueList.ServiceConfigs = inParam.ProjectConfig.ServiceConfigs;
            IssueList.PopulateIssueList();
            IssueList.CurrentConnectionStatus = () => IssueSearch.ConnectTo.Selected.Value as AppStatusBundle;
        }
    }
}
