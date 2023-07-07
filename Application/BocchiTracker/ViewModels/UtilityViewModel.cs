using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private ObservableCollection<PostServiceItem> _postServices = new ObservableCollection<PostServiceItem>();
        public ObservableCollection<PostServiceItem> PostServices
        {
            get => _postServices;
            set { SetProperty(ref _postServices, value); }
        }

        public UtilityViewModel()
        {
            PostServices.Add(new PostServiceItem { Name = nameof(BocchiTracker.Config.IssueServiceDefinitions.JIRA) });
            PostServices.Add(new PostServiceItem { Name = nameof(BocchiTracker.Config.IssueServiceDefinitions.Redmine) });
            PostServices.Add(new PostServiceItem { Name = nameof(BocchiTracker.Config.IssueServiceDefinitions.Github) });
            PostServices.Add(new PostServiceItem { Name = nameof(BocchiTracker.Config.IssueServiceDefinitions.Discord) });

            TakeScreenshotCommand   = new DelegateCommand(OnTakeScreenshot);
            CaptureCoredumpCommand  = new DelegateCommand(OnCaptureCoredump);
            PostIssueCommand        = new DelegateCommand(OnPostIssue);
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
