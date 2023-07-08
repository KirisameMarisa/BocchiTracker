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
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionToken;

        public ClassViewModel(IEventAggregator inEventAggregator)
        {
            HintText = "Class";

            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload()
        {
            var cachedConfigRepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var config = cachedConfigRepository.Load();

            foreach (var item in config.Classes)
            {
                base.Items.Add(item);
            }

            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionToken);
        }
    }
}
