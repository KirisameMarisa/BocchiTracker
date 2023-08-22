using BocchiTracker.Client.Config.Controls;
using BocchiTracker.Client.Share.Events;
using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientData.Configs;
using BocchiTracker.IssueInfoCollector;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BocchiTracker.Client.Config.ViewModels
{
    class MonitoredDirectories
    {
        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ReactiveCollection<string> Items { get; set; } = new ReactiveCollection<string>();

        public MonitoredDirectories(ProjectConfig inProjectConfig)
        {
            AddCommand = new DelegateCommand<Tuple<string, string>>(OnAddItem);
            RemoveCommand = new DelegateCommand<string>(OnRemoveItem);

            Items = new ReactiveCollection<string>(/*inIssueInfoBundle.TicketData.Lables*/);
            Items.CollectionChanged += (_, __) => 
            {
                inProjectConfig.MonitoredDirectoryConfigs = Items.Select(x =>
                {
                    var item = x.Split(",");
                    return new MonitoredDirectoryConfig { Directory = item[0], Filter = item[1] };
                }).ToList();
            };
        }

        public void OnAddItem(Tuple<string, string> inValue)
        {
            var newItem = $"{inValue.Item1}, {inValue.Item2}";
            if (Find(newItem) != null)
                return;
            Items.Add(newItem);
        }

        public void OnRemoveItem(string inValue)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var itemToRemove = Find(inValue);
                if (itemToRemove != null)
                {
                    Items.Remove(itemToRemove);
                }
            });
        }

        public string Find(string inValue)
        {
            return Items.FirstOrDefault(kvp => kvp == inValue);
        }
    }

    public class ExternalToolPathes
    {
        public ReactiveProperty<string> ProcdumpPath { get; set; }

        public ExternalToolPathes(ProjectConfig inProjectConfig)
        {
            ProcdumpPath = new ReactiveProperty<string>();
            ProcdumpPath.Subscribe(value => inProjectConfig.ExternalToolsPath.ProcDumpPath = value);
        }
    }

    public class FileSaveDirectory
    {
        public ReactiveProperty<string> WorkingDirectory { get; set; }
        public ReactiveProperty<string> CacheDirectory { get; set; }

        public FileSaveDirectory(ProjectConfig inProjectConfig)
        {
            WorkingDirectory = new ReactiveProperty<string>();
            WorkingDirectory.Subscribe(value => inProjectConfig.FileSaveDirectory = value);

            CacheDirectory = new ReactiveProperty<string>();
            CacheDirectory.Subscribe(value => inProjectConfig.CacheDirectory = value);
        }
    }

    class DirectoryViewModel : BindableBase
    {
        public MonitoredDirectories MonitoredDirectories { get; set; }
        public ExternalToolPathes ExternalToolPathes { get; set; }
        public FileSaveDirectory FileSaveDirectory { get; set; }

        public DirectoryViewModel(IEventAggregator inEventAggregator, ProjectConfig inProjectConfig) 
        {
            MonitoredDirectories        = new MonitoredDirectories(inProjectConfig);
            ExternalToolPathes          = new ExternalToolPathes(inProjectConfig);
            FileSaveDirectory           = new FileSaveDirectory(inProjectConfig);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            var config = inParam.ProjectConfig;

            foreach(var dir in config.MonitoredDirectoryConfigs) 
            {
                MonitoredDirectories.OnAddItem(new Tuple<string, string>(dir.Directory, dir.Filter));
            }
            ExternalToolPathes.ProcdumpPath.Value       = config.ExternalToolsPath.ProcDumpPath;
            FileSaveDirectory.WorkingDirectory.Value    = config.FileSaveDirectory;
            FileSaveDirectory.CacheDirectory.Value      = config.CacheDirectory;
        }

        private void OnSaveConfig()
        {
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigrepository.Load();

            projectConfig.MonitoredDirectoryConfigs.Clear();
            foreach (var item in MonitoredDirectories.Items)
            {
                var splits = item.Split(',');
                var moniteredDirectory = new MonitoredDirectoryConfig();

                moniteredDirectory.Directory = splits[0];
                moniteredDirectory.Filter = splits.Count() > 1 ? splits[1].Trim() : null;
                projectConfig.MonitoredDirectoryConfigs.Add(moniteredDirectory);
            }
            projectConfig.ExternalToolsPath.ProcDumpPath = ExternalToolPathes.ProcdumpPath.Value;
            projectConfig.CacheDirectory = FileSaveDirectory.CacheDirectory.Value;
            projectConfig.FileSaveDirectory = FileSaveDirectory.WorkingDirectory.Value;
        }
    }
}
