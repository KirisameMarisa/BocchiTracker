using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.Event;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Prism.Ioc;

namespace BocchiTracker.ViewModels
{
    public class PostServiceItem
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }

    public class UtilityViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private SubscriptionToken _subscriptionToken;

        public ICommand TakeScreenshotCommand { get; private set; }

        public ICommand CaptureCoredumpCommand { get; private set; }

        public ICommand PostIssueCommand { get; private set; }

        private ObservableCollection<PostServiceItem> _postServices = new ObservableCollection<PostServiceItem>();
        public ObservableCollection<PostServiceItem> PostServices
        {
            get => _postServices;
            set { SetProperty(ref _postServices, value); }
        }

        public UtilityViewModel(IEventAggregator inEventAggregator)
        {
            TakeScreenshotCommand   = new DelegateCommand(OnTakeScreenshot);
            CaptureCoredumpCommand  = new DelegateCommand(OnCaptureCoredump);
            PostIssueCommand        = new DelegateCommand(OnPostIssue);

            _eventAggregator = inEventAggregator;
            _subscriptionToken = _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload()
        {
            var cachedConfigRepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var config = cachedConfigRepository.Load();

            foreach (var item in config.ServiceConfigs)
            {
                PostServices.Add(new PostServiceItem { Name = item.Service.ToString() });
            }

            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionToken);
        }

        public void OnPostIssue()
        {
            foreach(var service in PostServices) 
            {
                Trace.TraceInformation($"{service.Name}, {service.IsSelected}");
            }
        }

        public void OnCaptureCoredump()
        {

        }

        public void OnTakeScreenshot()
        {

        }
    }
}
