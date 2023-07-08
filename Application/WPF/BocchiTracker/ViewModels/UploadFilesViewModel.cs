using BocchiTracker.Behaviors;
using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.Event;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public class UploadFileItem
    {
        public string Name { get; set; }

        public string FullName { get; set; }
    }

    public class UploadFilesViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _subscriptionConfigReloadEventToken;

        private IssueAssetsBundle _issueAssetsBundle;
        private ObservableCollection<UploadFileItem> _bundle;
        private List<IssueAssetMonitor> _issueAssetMonitors = new List<IssueAssetMonitor>();

        public ICommand OpenCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ObservableCollection<UploadFileItem> Bundle
        {
            get => _bundle;
            set => SetProperty(ref _bundle, value);
        }

        public UploadFilesViewModel(IEventAggregator inEventAggregator)
        {
            _issueAssetsBundle = (Application.Current as PrismApplication).Container.Resolve<IssueAssetsBundle>();
            Bundle = new ObservableCollection<UploadFileItem>();

            DeleteCommand = new DelegateCommand<string>(OnDeleteFile);
            OpenCommand = new DelegateCommand<string>(OnOpenFile);

            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<AssetDropedEvent>()
                .Subscribe(OnAddDroppedFiles, ThreadOption.UIThread);

            _subscriptionConfigReloadEventToken = _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload()
        {
            var cachedConfigRepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var config = cachedConfigRepository.Load();

            foreach(var item in config.MonitoredDirectoryConfigs)
            {
                if (string.IsNullOrEmpty(item.Directory) || !Directory.Exists(item.Directory))
                    continue;

                var issueAssetMonitor           = new IssueAssetMonitor(item.Directory, item.Filter);
                issueAssetMonitor.AddedAction   = OnAddFile;
                issueAssetMonitor.DeletedAction = OnDeleteFile;
                issueAssetMonitor.RenamedAction = OnRenameFile;
                _issueAssetMonitors.Add(issueAssetMonitor);
            }
            
            _eventAggregator
                .GetEvent<IssueInfoLoadCompleteEvent>()
                .Unsubscribe(_subscriptionConfigReloadEventToken);
        }

        public void OnAddDroppedFiles(AssetDropedEventParameter inParameter)
        {
            foreach(var file in inParameter.Files)
            {
                OnAddFile(file);
            }
        }

        public void OnAddFile(string inFilePath)
        {
            _issueAssetsBundle.Add(inFilePath);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Bundle.Add(new UploadFileItem { FullName = inFilePath, Name = Path.GetFileName(inFilePath) });
            });
        }

        public void OnDeleteFile(string inFilePath)
        {
            if (_issueAssetsBundle.Delete(inFilePath))
            {
                var removeItem = Bundle.Where(x => x.FullName == inFilePath).FirstOrDefault() ?? null;
                if (removeItem != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Bundle.Remove(removeItem);
                    });
                }
            }
        }

        public void OnRenameFile(string inOldPath, string inNewPath)
        {
            OnDeleteFile(inOldPath);
            OnAddFile(inNewPath);
        }

        public void OnOpenFile(string inFilePath)
        {
            //!< TODO::
        }
    }
}
