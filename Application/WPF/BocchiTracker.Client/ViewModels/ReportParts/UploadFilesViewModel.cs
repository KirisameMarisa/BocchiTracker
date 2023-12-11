using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
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
using System.Diagnostics;
using BocchiTracker.ModelEvent;
using System.Windows.Media.Imaging;

namespace BocchiTracker.Client.ViewModels.ReportParts
{
    public class UploadItem
    {
        public ICommand EditCommand { get; private set; }

        public ICommand RemoveCommand { get; private set; }

        public bool IsSelected { get; set; } = false;

        public AssetData AssetData { get; set; }

        public ReactiveProperty<BitmapImage> PreviewImage { get; set; } = new ReactiveProperty<BitmapImage>();

        private UploadFilesViewModel _viewModel;

        public UploadItem(AssetData inAssetData, UploadFilesViewModel inViewModel)
        {
            AssetData = inAssetData;
            
            if (AssetData.IsPreviewPictureSupport())
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    PreviewImage.Value = Load(AssetData.PreviewLoadingRawData);
                });

                Task.Run(async () =>
                {
                    while (!AssetData.IsPreviewPictureLoadCompleted())
                        await Task.Delay(10000);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        PreviewImage.Value = Load(AssetData.PictureRawData);
                    });
                });
            }

            _viewModel = inViewModel;

            RemoveCommand = new DelegateCommand(() => _viewModel.OnRemoveFile(AssetData.FullName));
            EditCommand = new DelegateCommand(OnEditFile);
        }

        private void OnEditFile()
        {
            var info = new ProcessStartInfo(AssetData.FullName) { UseShellExecute = true };
            Process.Start(info);
        }

        private BitmapImage Load(byte[] inPictureRawData)
        {
            var memStream = new MemoryStream(inPictureRawData);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memStream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
    }

    public class UploadFilesViewModel : BindableBase
    {
        private IssueAssetsBundle _issueAssetsBundle;

        private List<IssueAssetMonitor> _issueAssetMonitors = new List<IssueAssetMonitor>();

        public ReactiveProperty<bool> IsShowHelperText { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveCollection<UploadItem> Bundle { get; }

        public UploadFilesViewModel(IEventAggregator inEventAggregator, IssueAssetsBundle inIssueAssetsBundle)
        {
            _issueAssetsBundle = inIssueAssetsBundle;

            Bundle = new ReactiveCollection<UploadItem>();
            Bundle.CollectionChanged += (_, __) => OnUpdateCollection();

            inEventAggregator
                .GetEvent<AssetDropedEvent>()
                .Subscribe(OnAddDroppedFiles, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<IssueSubmittedEvent>()
                .Subscribe(OnIssueSubmittedEvent, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            var monitoredDirectories = inParam.ProjectConfig.MonitoredDirectoryConfigs;
            monitoredDirectories.Add(new MonitoredDirectoryConfig { Directory = inParam.ProjectConfig.FileSaveDirectory, Filter = "*" });
            foreach (var item in monitoredDirectories)
            {
                if (string.IsNullOrEmpty(item.Directory) || !Directory.Exists(item.Directory))
                    continue;

                var issueAssetMonitor           = new IssueAssetMonitor(item.Directory, item.Filter);
                issueAssetMonitor.AddedAction   = OnAddFile;
                issueAssetMonitor.DeletedAction = OnRemoveFile;
                issueAssetMonitor.RenamedAction = OnRenameFile;
                _issueAssetMonitors.Add(issueAssetMonitor);
            }

            if(inParam.UserConfig != null)
            {
                foreach (var item in inParam.UserConfig.DraftUploadFiles)
                {
                    OnAddFile(item);
                }
            }
        }

        private void OnIssueSubmittedEvent(IssueSubmittedEventParameter inParam)
        {
            Bundle.Clear();
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
            if (!Path.Exists(inFilePath))
                return;
            Bundle.AddOnScheduler(new UploadItem(new AssetData(inFilePath), this));
        }

        public void OnRemoveFile(string inFilePath)
        {
            var removeItem = Bundle.Where(x => x.AssetData.FullName == inFilePath).FirstOrDefault() ?? null;
            if (removeItem != null)
            {
                Bundle.RemoveOnScheduler(removeItem);
            }
        }

        public void OnRenameFile(string inOldPath, string inNewPath)
        {
            OnRemoveFile(inOldPath);
            OnAddFile(inNewPath);
        }

        public void OnUpdateCollection()
        {
            _issueAssetsBundle.Bundle = Bundle.Select(x => x.AssetData).ToList();
            
            IsShowHelperText.Value = Bundle.Count == 0 ? true : false;
        }
    }
}
