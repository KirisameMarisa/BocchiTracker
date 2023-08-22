using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientData.Configs;
using BocchiTracker.Client.Share.Events;
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
using Reactive.Bindings;
using System.IO.Abstractions;
using BocchiTracker.ModelEvent;
using BocchiTracker.Client.Controls;

namespace BocchiTracker.Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        public ICommand LocationChangedCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand ActiveChangedCommand { get; }
        public ICommand DropFilesCommand { get; private set; }
        public ICommand ClosedCommand { get; private set; }

        public ReactiveProperty<bool> IsSubmitting { get; set; }

        public MainWindowViewModel(IEventAggregator inEventAggregator) 
        {
            _eventAggregator = inEventAggregator;

            DropFilesCommand        = new DelegateCommand<string[]>(OnDropFiles);
            ClosedCommand           = new DelegateCommand(OnCloseCommand);
            MouseMoveCommand        = new DelegateCommand(OnMouseMove);
            ActiveChangedCommand    = new DelegateCommand<string>(OnActiveChanged);
            LocationChangedCommand  = new DelegateCommand(OnLocationChanged);

            IsSubmitting            = new ReactiveProperty<bool>(false);

            _eventAggregator.GetEvent<IssueSubmitPreEvent>().Subscribe( () => { IsSubmitting.Value = true;  });
            _eventAggregator.GetEvent<IssueSubmittedEvent>().Subscribe(  _ => { IsSubmitting.Value = false; });
        }

        private void OnDropFiles(string[] inFiles)
        {
            _eventAggregator
                .GetEvent<AssetDropedEvent>()
                .Publish(new AssetDropedEventParameter(inFiles));
        }


        private void OnLocationChanged()
        {
            _eventAggregator
                .GetEvent<WindowLocationChangedEvent>()
                .Publish();
        }

        private void OnMouseMove()
        {
            _eventAggregator
                .GetEvent<WindowMouseMoveEvent>()
                .Publish();
        }

        private void OnActiveChanged(string inState)
        {
            if(bool.TryParse(inState, out bool outState)) 
            {
                _eventAggregator
                    .GetEvent<WindowActiveChangedEvent>()
                    .Publish(outState);
            }
        }

        public void OnCloseCommand()
        {
            var issueInfoBundle         = (Application.Current as PrismApplication).Container.Resolve<IssueInfoBundle>();
            var issueAssetsBundle       = (Application.Current as PrismApplication).Container.Resolve<IssueAssetsBundle>();
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var userConfigrepository    = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<UserConfig>>();

            UserConfig config = new UserConfig();
            config.ProjectConfigFilename    = projectConfigrepository.GetLoadFilename();
            config.DraftUploadFiles         = issueAssetsBundle.Bundle.Select(x => x.FullName).ToList();
            config.DraftTicketData          = issueInfoBundle.TicketData;
            config.SelectedService          = issueInfoBundle.PostServices.Select(x => x.ToString()).ToList();
            userConfigrepository.Save(config);
        }
    }
}
