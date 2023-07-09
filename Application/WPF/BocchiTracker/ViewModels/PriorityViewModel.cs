using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.Event;
using BocchiTracker.IssueInfoCollector;
using MaterialDesignThemes.Wpf;
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
    public class PriorityViewModel : SingleItemViewModel
    {
        private IssueInfoBundle _issueInfoBundle;

        public PriorityViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            HintText.Value = "Priority";

            _issueInfoBundle = inIssueInfoBundle;

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        protected override void OnSetSelectedItem(string inItem)
        {
            _issueInfoBundle.TicketData.Priority = inItem;
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.Priorities)
            {
                base.Items.Add(item);
            }
        }
    }
}
