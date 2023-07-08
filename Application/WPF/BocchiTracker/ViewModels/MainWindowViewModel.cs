using BocchiTracker.Event;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Redmine.Net.Api.Extensions;
using Slack.NetStandard.EventsApi.CallbackEvents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace BocchiTracker.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        public ICommand DropFilesCommand { get; private set; }

        public MainWindowViewModel(IEventAggregator inEventAggregator) 
        {
            _eventAggregator = inEventAggregator;
            DropFilesCommand = new DelegateCommand<string[]>(OnDropFiles);
        }

        private void OnDropFiles(string[] inFiles)
        {
            _eventAggregator
                .GetEvent<AssetDropedEvent>()
                .Publish(new AssetDropedEventParameter(inFiles));
        }
    }
}
