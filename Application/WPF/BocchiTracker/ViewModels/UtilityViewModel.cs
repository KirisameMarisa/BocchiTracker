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
using Reactive.Bindings;
using BocchiTracker.IssueInfoCollector;

namespace BocchiTracker.ViewModels
{
    public class PostServiceItem
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }

    public class UtilityViewModel : BindableBase
    {
        public ICommand TakeScreenshotCommand { get; private set; }

        public ICommand CaptureCoredumpCommand { get; private set; }

        public ICommand PostIssueCommand { get; private set; }

        public ReactiveCollection<PostServiceItem> PostServices { get; }

        public UtilityViewModel(IEventAggregator inEventAggregator, IssueInfoBundle inIssueInfoBundle)
        {
            TakeScreenshotCommand   = new DelegateCommand(OnTakeScreenshot);
            CaptureCoredumpCommand  = new DelegateCommand(OnCaptureCoredump);
            PostIssueCommand        = new DelegateCommand(OnPostIssue);
            PostServices            = new ReactiveCollection<PostServiceItem>();

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.ServiceConfigs)
                PostServices.Add(new PostServiceItem { Name = item.Service.ToString() });
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
