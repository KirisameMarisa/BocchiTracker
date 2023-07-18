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
using BocchiTracker.CrossServiceUploader;
using BocchiTracker.Data;
using BocchiTracker.UIHelpers.Commands;

namespace BocchiTracker.ViewModels
{
    public class UtilityViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

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
        private readonly IIssueAssetUploader _issueAssetUploader;
        private ProjectConfig _projectConfig;
        private UserConfig _userConfig;

        public UtilityViewModel(IEventAggregator inEventAggregator, IIssuePoster inIssuePoster, IIssueAssetUploader inIssueAssetUploader, ICreateActionHandler inCreateActionHandler, IssueInfoBundle inIssueInfoBundle, IssueAssetsBundle inIssueAssetsBundle, AppStatusBundles inAppStatusBundles)
        {
            TakeScreenshotCommand   = new DelegateCommand(OnTakeScreenshot);
            CaptureCoredumpCommand  = new DelegateCommand(OnCaptureCoredump);
            PostIssueCommand        = new AsyncCommand(OnPostIssue);

            PostServices            = new ReactiveCollection<PostServiceItem>();

            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            _createActionHandler = inCreateActionHandler;

            _issueInfoBundle = inIssueInfoBundle;
            _issueAssetsBundle = inIssueAssetsBundle;
            _appStatusBundles = inAppStatusBundles;
            _issuePoster = inIssuePoster;
            _issueAssetUploader = inIssueAssetUploader;
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            _userConfig = inParam.UserConfig;
            _projectConfig = inParam.ProjectConfig;
            
            foreach (var item in _projectConfig.ServiceConfigs)
            {
                var serviceItem = new PostServiceItem { 
                    Name = new ReactiveProperty<string>(item.Service.ToString()),
                    IsSelected = new ReactiveProperty<bool>(false)
                };
                serviceItem.IsSelected.Subscribe(_ => OnChangedPostService());
                PostServices.Add(serviceItem);
            }

            foreach (var item in PostServices)
            {
                if (_userConfig != null)
                {
                    if (_userConfig.SelectedService.Contains(item.Name.Value))
                        item.IsSelected.Value = true;
                }
            }
            _eventAggregator.GetEvent<IssuePostedEvent>().Publish();
        }

        public void OnChangedPostService()
        {
            _issueInfoBundle.PostServices = PostServices
                .Where(x => x.IsSelected.Value)
                .Select(x => Enum.Parse<ServiceDefinitions>(x.Name.Value))
                .ToList();
        }

        public async Task OnPostIssue()
        {
            foreach(var service in _issueInfoBundle.PostServices) 
            {
                string key = await _issuePoster.Post(service, _issueInfoBundle, _appStatusBundles.TrackerApplication, _projectConfig);
                Trace.TraceInformation($"{service}, {key}");

                if (key == null)
                    continue;

                await _issueAssetUploader.Upload(service, key, _issueAssetsBundle, _projectConfig);
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
