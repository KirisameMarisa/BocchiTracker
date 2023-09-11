using BocchiTracker.Client.ViewModels.ReportParts;
using BocchiTracker.Data;
using BocchiTracker.ModelEvent;
using BocchiTracker.ServiceClientData;
using Prism.Events;
using Reactive.Bindings;
using System;

namespace BocchiTracker.Client.ViewModels.IssueListParts
{
    public class SearchPart
    {
        public bool IsVisible { get; set; } = true;

        public ConnectTo ConnectTo { get; set; }

        public ReactiveProperty<string> SearchText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> ShowItemsWithLocation { get; set; } = new ReactiveProperty<bool>(false);

        public IssueListParts.ListPart IssueList = default!;

        public SearchPart(IEventAggregator inEventAggregator, TicketProperty inTicketProperty, ListPart inIssueList)
        {
            IssueList = inIssueList;
            ConnectTo = new ConnectTo(inTicketProperty);

            SearchText.Subscribe((_) => OnApplyFilterIssues());
            ShowItemsWithLocation.Subscribe((_) => OnApplyFilterIssues());

            inEventAggregator
                .GetEvent<SummarySearchEvent>()
                .Subscribe(OnSearchFromEvent, ThreadOption.UIThread);
        }

        private void OnApplyFilterIssues()
        {
            var filterText = SearchText.Value;
            var showItemsWithLocation = ShowItemsWithLocation.Value;

            IssueList.Clear();

            var issues = IssueList.GetAllIssues();
            foreach (var issue in issues)
            {
                if (issue.Contatins(TicketFilter.Summary | TicketFilter.Id | TicketFilter.Assign, filterText))
                {
                    if (showItemsWithLocation)
                    {
                        if (issue.CanJumpPlayer())
                            IssueList.Add(issue);
                    }
                    else
                    {
                        IssueList.Add(issue);
                    }
                }
            }
        }

        private void OnSearchFromEvent(string inText)
        {
            if (IsVisible)
                return;
            SearchText.Value = inText;
        }
    }
}
