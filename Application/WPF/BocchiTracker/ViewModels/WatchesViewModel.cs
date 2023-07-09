using BocchiTracker.Event;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BocchiTracker.ViewModels
{
    public class WatchesViewModel : MultipleItemsViewModel
    {
        private IssueInfoBundle _issueInfoBundle;

        public WatchesViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            HintText.Value = "Watches";

            _issueInfoBundle = inIssueInfoBundle;

            inEventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Subscribe(OnIssueInfoLoadComplete, ThreadOption.UIThread);
        }

        protected override void OnUpdateRegisteredItems()
        {
            var registeredItems = new ObservableCollection<string>(RegisteredItems);
            _issueInfoBundle.TicketData.Watcheres = registeredItems.ToList();
        }

        private void OnIssueInfoLoadComplete()
        {
            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            foreach (var user in issue_info_bundle.UserListService.GetUnifiedData())
            {
                base.Items.Add(user.Name);
            }
        }
    }
}
