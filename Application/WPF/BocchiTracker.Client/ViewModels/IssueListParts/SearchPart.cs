using BocchiTracker.Data;
using BocchiTracker.ServiceClientData;
using Reactive.Bindings;
using System;

namespace BocchiTracker.Client.ViewModels.IssueListParts
{
    public class SearchPart
    {
        public ReactiveProperty<bool> IsVisible { get; set; } = new ReactiveProperty<bool>(true);

        public ReactiveCollection<ServiceDefinitions> ServiceDefinitions { get; set; } = new ReactiveCollection<ServiceDefinitions>();
        public ReactiveProperty<ServiceDefinitions> SelectedService { get; set; } = new ReactiveProperty<ServiceDefinitions>();
        public ConnectTo ConnectTo { get; set; }

        public ReactiveProperty<string> SearchText { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> ShowItemsWithLocation { get; set; } = new ReactiveProperty<bool>(false);

        public IssueListParts.ListPart IssueList = default!;

        public SearchPart(TicketProperty inTicketProperty, ListPart inIssueList)
        {
            IssueList = inIssueList;
            ConnectTo = new ConnectTo(inTicketProperty);

            SelectedService.Subscribe((service) => IssueList.PopulateIssueList(service));
            ServiceDefinitions.CollectionChanged += (_, __) =>
            {
                if (ServiceDefinitions.Count == 1)
                    SelectedService.Value = ServiceDefinitions[0];
            };

            SearchText.Subscribe((_) => OnApplyFilterIssues());
            ShowItemsWithLocation.Subscribe((_) => OnApplyFilterIssues());
        }

        private void OnApplyFilterIssues()
        {
            var filterText = SearchText.Value;
            var showItemsWithLocation = ShowItemsWithLocation.Value;

            IssueList.Clear();

            var issues = IssueList.GetAllIssues(SelectedService.Value);
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
    }
}
