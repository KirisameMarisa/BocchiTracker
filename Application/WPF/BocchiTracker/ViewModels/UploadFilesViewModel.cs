using BocchiTracker.Behaviors;
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
        private IssueAssetsBundle _issueAssetsBundle;
        private ObservableCollection<UploadFileItem> _bundle;

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

            inEventAggregator
                .GetEvent<AssetDropedEvent>()
                .Subscribe(OnAddDroppedFiles, ThreadOption.UIThread);
        }

        public void OnAddDroppedFiles(AssetDropedEventParameter inParameter)
        {
            foreach(var file in inParameter.Files)
            {
                _issueAssetsBundle.Add(file);
                Bundle.Add(new UploadFileItem { FullName = file, Name = Path.GetFileName(file) });
            }
        }

        public void OnDeleteFile(string inFilePath)
        {
            if (_issueAssetsBundle.Delete(inFilePath))
            {
                var removeItem = Bundle.Where(x => x.FullName == inFilePath).FirstOrDefault() ?? null;
                if (removeItem != null)
                {
                    Bundle.Remove(removeItem);
                }
            }
        }

        public void OnOpenFile(string inFilePath)
        {
            //!< TODO::
        }
    }
}
