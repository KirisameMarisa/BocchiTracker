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
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionToken;

        public PriorityViewModel(IEventAggregator inEventAggregator)
        {
            HintText = "Priority";

            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload()
        {
            var cachedConfigRepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var config = cachedConfigRepository.Load();

            foreach (var item in config.Priorities)
            {
                base.Items.Add(item);
            }

            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionToken);
        }
    }
}
