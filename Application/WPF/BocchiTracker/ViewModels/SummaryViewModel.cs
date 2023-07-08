using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Ioc;
using BocchiTracker.Event;
using Prism.Events;

namespace BocchiTracker.ViewModels
{
    public class SummaryViewModel : BindableBase
    {
        private string _ticket_type;
        public string TicketType
        {
            get => _ticket_type;
            set { SetProperty(ref _ticket_type, value); }
        }

        private string _summary;
        public string Summary
        {
            get => _summary;
            set { SetProperty(ref _summary, value); }
        }

        private string _selected_ticket_type;
        public string SelectedTicketType
        {
            get => _selected_ticket_type;
            set { SetProperty(ref _selected_ticket_type, value); }
        }

        private ObservableCollection<string> _ticketTypes = new ObservableCollection<string>();
        public ObservableCollection<string> TicketTypes
        {
            get => _ticketTypes;
            set { SetProperty(ref _ticketTypes, value); }
        }

        private IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionToken;

        public SummaryViewModel(IEventAggregator inEventAggregator)
        {
            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload()
        {
            var cachedConfigRepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var config = cachedConfigRepository.Load();

            foreach (var item in config.TicketTypes)
            {
                TicketTypes.Add(item);
            }

            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionToken);
        }
    }
}
