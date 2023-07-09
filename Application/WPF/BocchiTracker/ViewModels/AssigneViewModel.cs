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
    public class AssigneViewModel : SingleItemViewModel
    {
        private IssueInfoBundle _issueInfoBundle;

        public AssigneViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            HintText.Value = "Assigne";

            _issueInfoBundle = inIssueInfoBundle;

            inEventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Subscribe(OnIssueInfoLoadComplete, ThreadOption.UIThread);
        }

        protected override void OnSetSelectedItem(string inItem)
        {
            _issueInfoBundle.TicketData.Assignee = inItem;
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
