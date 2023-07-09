using BocchiTracker.Event;
using BocchiTracker.IssueInfoCollector;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BocchiTracker.ViewModels
{
    public class LabelsViewModel : MultipleItemsViewModel
    {
        private IssueInfoBundle _issueInfoBundle;

        public LabelsViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle) 
        {
            HintText.Value = "Labels";

            _issueInfoBundle = inIssueInfoBundle;

            inEventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Subscribe(OnIssueInfoLoadComplete, ThreadOption.UIThread);
        }

        protected override void OnUpdateRegisteredItems()
        {
            var registeredItems = new ObservableCollection<string>(RegisteredItems);
            _issueInfoBundle.TicketData.Lables = registeredItems.ToList();
        }

        private void OnIssueInfoLoadComplete()
        {
            var issue_info_bundle = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            foreach (var label in issue_info_bundle.LabelListService.GetUnifiedData())
            {
                base.Items.Add(label.Name);
            }
        }
    }
}
