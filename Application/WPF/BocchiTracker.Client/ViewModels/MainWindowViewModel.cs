using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
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
using BocchiTracker.Config;

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

        public ReactiveProperty<bool>   IsProgressing { get; set; }
        public ReactiveCollection<string> ProgressResonMsg { get; set; }

        public MainWindowViewModel(IEventAggregator inEventAggregator) 
        {
            _eventAggregator = inEventAggregator;

            DropFilesCommand        = new DelegateCommand<string[]>(OnDropFiles);
            ClosedCommand           = new DelegateCommand(OnCloseCommand);
            MouseMoveCommand        = new DelegateCommand(OnMouseMove);
            ActiveChangedCommand    = new DelegateCommand<string>(OnActiveChanged);
            LocationChangedCommand  = new DelegateCommand(OnLocationChanged);

            IsProgressing           = new ReactiveProperty<bool>(false);
            ProgressResonMsg        = new ReactiveCollection<string>();
            ProgressResonMsg.CollectionChanged += (_, __) => 
            {
                if (ProgressResonMsg.Count > 3)
                    ProgressResonMsg.RemoveAtOnScheduler(0);
            };

            _eventAggregator.GetEvent<StartProgressEvent>().Subscribe(
                inParam => { 
                    IsProgressing.Value = true; 
                    if (!string.IsNullOrEmpty(inParam.Message)) 
                        ProgressResonMsg.Add(inParam.Message); 
                }, ThreadOption.UIThread);
            
            _eventAggregator.GetEvent<EndProgressEvent>().Subscribe(
                () => { 
                    IsProgressing.Value = false; 
                }, ThreadOption.UIThread);
            
            _eventAggregator.GetEvent<ProgressingEvent>().Subscribe(
                inParam => { 
                    if(!string.IsNullOrEmpty(inParam.Message)) 
                        ProgressResonMsg.Add(inParam.Message); 
                }, ThreadOption.UIThread);
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
