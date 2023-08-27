using BocchiTracker.CrossServiceReporter;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Client.ViewModels
{
    public class IssuesViewModel : BindableBase
    {
        public ReactiveProperty<ServiceDefinitions> ServiceDefinitionsProperty { get; set; } = new ReactiveProperty<ServiceDefinitions>();
        public ReactiveCollection<TicketData> Issus { get; set; } = new ReactiveCollection<TicketData>();

        private IEventAggregator _eventAggregator;
        private IssueInfoBundle _issueBundle;
        private IGetIssues _getIssues;

        public IssuesViewModel(IEventAggregator inEventAggregator, IGetIssues inGetIssues, IssueInfoBundle inIssueBundle)
        {
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            _getIssues = inGetIssues;
            _issueBundle = inIssueBundle;

            
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            Task.Run(async () =>
            {
                var issues = _getIssues.Get(ServiceDefinitions.Github, _issueBundle.CustomFieldsListService as CustomFieldListService);
                await foreach (var issue in issues)
                {
                    Issus.AddOnScheduler(issue);
                }
            });
        }
    }
}
