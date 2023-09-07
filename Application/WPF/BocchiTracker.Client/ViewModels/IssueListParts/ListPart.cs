﻿using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.ServiceClientData;
using Prism.Commands;
using Prism.Events;
using Reactive.Bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BocchiTracker.Client.ViewModels.IssueListParts
{
    public class ListPart
    {
        public Func<AppStatusBundle> CurrentConnectionStatus { get; set; }
        public Func<ServiceDefinitions> CurrentService { get; set; }

        public ICommand OpenInBrowserCommand { get; private set; }

        public ReactiveCollection<IssueListParts.IssueItem> Issues { get; set; } = new ReactiveCollection<IssueListParts.IssueItem>();
        public IEnumerable SelectedIssues { get; set; } = new ObservableCollection<IssueListParts.IssueItem>();

        public Dictionary<ServiceDefinitions, ServiceConfig> ServiceDefinitions { get; set; } = new Dictionary<ServiceDefinitions, ServiceConfig>();

        private readonly IGetIssues _getIssues;
        private readonly IEventAggregator _eventAggregator;
        private readonly IIssueOpener _issueOpener;

        public ListPart(IGetIssues inGetIssues, IEventAggregator inEventAggregator, IIssueOpener inIssueOpener)
        {
            OpenInBrowserCommand = new DelegateCommand(OnOpenInBrowser);

            _getIssues = inGetIssues;
            _eventAggregator = inEventAggregator;
            _issueOpener = inIssueOpener;
        }

        private void OnOpenInBrowser()
        {
            foreach (var item in SelectedIssues)
            {
                var obj = item as IssueListParts.IssueItem;
                if (obj == null)
                    continue;
                obj.OnOpenInBrowser();
            }
        }

        public void PopulateIssueList(ServiceDefinitions inService)
        {
            this.Issues.Clear();

            var issues = this.GetAllIssues(inService);
            foreach (var issue in issues)
            {
                Add(issue);
            }
        }

        public List<TicketData> GetAllIssues(ServiceDefinitions inService)
        {
            if (!ServiceDefinitions.ContainsKey(inService))
                return new List<TicketData>();
            return _getIssues.GetFromCache(ServiceDefinitions[inService]);
        }

        public void Clear()
        {
            Issues.Clear();
        }

        public void Add(TicketData inTicketData)
        {
            Issues.AddOnScheduler(new IssueListParts.IssueItem(inTicketData, _eventAggregator, _issueOpener)
            {
                CurrentService = CurrentService,
                CurrentConnectionStatus = CurrentConnectionStatus
            });
        }
    }
}
