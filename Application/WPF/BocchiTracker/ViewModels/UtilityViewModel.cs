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
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.IssueAssetCollector.Handlers;
using BocchiTracker.IssueAssetCollector.Handlers.Coredump;
using System.Net;
using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using BocchiTracker.CrossServiceReporter;
using System.ComponentModel.DataAnnotations;
using Reactive.Bindings.Extensions;

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

        [Required(ErrorMessage = "Required")]
        public ReactiveCollection<PostServiceItem> PostServices { get; }

        private readonly ICreateActionHandler _createActionHandler;
        private readonly IssueInfoBundle _issueInfoBundle;
        private readonly IssueAssetsBundle _issueAssetsBundle;
        private readonly AppStatusBundles _appStatusBundles;
        private readonly IIssuePoster _issuePoster;
        private ProjectConfig _projectConfig;

        public UtilityViewModel(IEventAggregator inEventAggregator, IIssuePoster inIssuePoster, ICreateActionHandler inCreateActionHandler, IssueInfoBundle inIssueInfoBundle, IssueAssetsBundle inIssueAssetsBundle, AppStatusBundles inAppStatusBundles)
        {
            TakeScreenshotCommand   = new DelegateCommand(OnTakeScreenshot);
            CaptureCoredumpCommand  = new DelegateCommand(OnCaptureCoredump);
            PostIssueCommand        = new DelegateCommand(OnPostIssue);
            PostServices            = new ReactiveCollection<PostServiceItem>();

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            _createActionHandler = inCreateActionHandler;

            _issueInfoBundle = inIssueInfoBundle;
            _issueAssetsBundle = inIssueAssetsBundle;
            _appStatusBundles = inAppStatusBundles;
            _issuePoster = inIssuePoster;
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            _projectConfig = inParam.ProjectConfig;
            foreach (var item in _projectConfig.ServiceConfigs)
                PostServices.Add(new PostServiceItem { Name = item.Service.ToString() });
        }

        public void OnPostIssue()
        {
            foreach(var service in PostServices) 
            {
                if(service.IsSelected)
                {
                    ServiceDefinitions serviceEnum = Enum.Parse<ServiceDefinitions>(service.Name);
                    _issuePoster.Post(serviceEnum, _issueInfoBundle, _appStatusBundles.TrackerApplication, _projectConfig);
                    Trace.TraceInformation($"{service.Name}, {service.IsSelected}");
                }
            }
        }

        public void OnCaptureCoredump()
        {
            if (_appStatusBundles.TrackerApplication == null)
                return;
#if WINDOWS
            if (int.TryParse(_appStatusBundles.TrackerApplication.AppBasicInfo.Pid, out int outPid))
            {
                var handler = _createActionHandler.Create(typeof(WindowsCoredumpHandler));
                handler.Handle(0, outPid, _projectConfig.FileSaveDirectory);
            }
#endif
        }

        public void OnTakeScreenshot()
        {
            if (_appStatusBundles.TrackerApplication == null)
                return;

            if (IPAddress.Parse("127.0.0.1").GetHashCode() == _appStatusBundles.TrackerApplication.AppBasicInfo.ClientID)
            {
                if(int.TryParse(_appStatusBundles.TrackerApplication.AppBasicInfo.Pid, out int outPid ))
                {
                    var handler = _createActionHandler.Create(typeof(LocalScreenshotHandler));
                    handler.Handle(0, outPid, _projectConfig.FileSaveDirectory);
                }
            }
            else
            {
                var handler = _createActionHandler.Create(typeof(RemoteScreenshotHandler));
                handler.Handle(_appStatusBundles.TrackerApplication.AppBasicInfo.ClientID, 0, _projectConfig.FileSaveDirectory);
            }
        }
    }
}
