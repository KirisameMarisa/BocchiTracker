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
using BocchiTracker.Event;
using Slack.NetStandard.EventsApi;
using BocchiTracker.Config.Configs;
using BocchiTracker.Config;

namespace BocchiTracker.ViewModels
{
    public class ClassViewModel : SingleItemViewModel
    {
        private IssueInfoBundle _issueInfoBundle;

        public ClassViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            HintText.Value = "Class";

            _issueInfoBundle = inIssueInfoBundle;

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        protected override void OnSetSelectedItem(string inItem)
        {
            _issueInfoBundle.TicketData.Class = inItem;
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.Classes)
            {
                base.Items.Add(item);
            }
        }
    }
}
