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
using Reactive.Bindings;
using System.Reflection.Metadata;

namespace BocchiTracker.ViewModels
{
    public class UploadFilesViewModel : BindableBase
    {
        private List<IssueAssetMonitor> _issueAssetMonitors = new List<IssueAssetMonitor>();

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public ReactiveCollection<AssetData> Bundle { get; }

        private IssueAssetsBundle _issueAssetsBundle;

        public UploadFilesViewModel(IEventAggregator inEventAggregator, IssueAssetsBundle inIssueAssetsBundle)
        {
            _issueAssetsBundle = inIssueAssetsBundle;

            Bundle = new ReactiveCollection<AssetData>();
            Bundle.CollectionChanged += (_, __) => OnUpdateCollection();
            DeleteCommand = new DelegateCommand<string>(OnDeleteFile);
            OpenCommand = new DelegateCommand<string>(OnOpenFile);

            inEventAggregator
                .GetEvent<AssetDropedEvent>()
                .Subscribe(OnAddDroppedFiles, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach(var item in inParam.ProjectConfig.MonitoredDirectoryConfigs)
            {
                if (string.IsNullOrEmpty(item.Directory) || !Directory.Exists(item.Directory))
                    continue;

                var issueAssetMonitor           = new IssueAssetMonitor(item.Directory, item.Filter);
                issueAssetMonitor.AddedAction   = OnAddFile;
                issueAssetMonitor.DeletedAction = OnDeleteFile;
                issueAssetMonitor.RenamedAction = OnRenameFile;
                _issueAssetMonitors.Add(issueAssetMonitor);
            }
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
            Bundle.AddOnScheduler(new AssetData(inFilePath));
        }

        public void OnDeleteFile(string inFilePath)
        {
            var removeItem = Bundle.Where(x => x.FullName == inFilePath).FirstOrDefault() ?? null;
            if (removeItem != null)
            {
                Bundle.RemoveOnScheduler(removeItem);
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

        public void OnUpdateCollection()
        {
            _issueAssetsBundle.Bundle = Bundle.ToList();
        }
    }
}
