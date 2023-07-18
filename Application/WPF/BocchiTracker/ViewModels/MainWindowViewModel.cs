using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.Event;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Redmine.Net.Api.Extensions;
using Slack.NetStandard.EventsApi.CallbackEvents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        public ICommand DropFilesCommand { get; private set; }
        public ICommand ClosedCommand { get; private set; }

        public MainWindowViewModel(IEventAggregator inEventAggregator) 
        {
            _eventAggregator = inEventAggregator;
            DropFilesCommand = new DelegateCommand<string[]>(OnDropFiles);
            ClosedCommand = new DelegateCommand(OnCloseCommand);
        }

        private void OnDropFiles(string[] inFiles)
        {
            _eventAggregator
                .GetEvent<AssetDropedEvent>()
                .Publish(new AssetDropedEventParameter(inFiles));
        }

        public void OnCloseCommand()
        {
            var issueInfoBundle     = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            var issueAssetsBundle   = (Application.Current as PrismApplication).Container.Resolve<IssueAssetsBundle>();
            var repository          = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<UserConfig>>();

            UserConfig config = new UserConfig();
            config.DraftUploadFiles = issueAssetsBundle.Bundle.Select(x => x.FullName).ToList();
            config.DraftTicketData = issueInfoBundle.TicketData;
            config.SelectedService = issueInfoBundle.PostServices.Select(x => x.ToString()).ToList();
            repository.Save(config);
        }
    }
}
