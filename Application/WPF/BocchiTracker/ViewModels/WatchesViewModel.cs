using BocchiTracker.Event;
using BocchiTracker.IssueInfoCollector;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BocchiTracker.ViewModels
{
    public class WatchesViewModel : MultipleItemsViewModel
    {
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionToken;

        public WatchesViewModel(IEventAggregator inEventAggregator)
        {
            HintText = "Watches";

            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Subscribe(OnIssueInfoLoadComplete, ThreadOption.UIThread);
        }

        private void OnIssueInfoLoadComplete()
        {
            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            foreach (var user in issue_info_bundle.UserListService.GetUnifiedData())
            {
                base.Items.Add(user.Name);
            }

            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionToken);
        }
    }
}
