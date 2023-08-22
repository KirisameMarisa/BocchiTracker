using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BocchiTracker.Config;
using BocchiTracker.ModelEvent;

namespace BocchiTracker.Client.Config.ViewModels
{
    class NetworkViewModel : BindableBase
    {
        [Range(1024, 65535, ErrorMessage = "Please enter value in 1024~65535")]
        public ReactiveProperty<string> TcpPort { get; set; }

        public NetworkViewModel(IEventAggregator inEventAggregator, ProjectConfig inProjectConfig)
        {
            TcpPort                     = new ReactiveProperty<string>("8888").SetValidateAttribute(() => this.TcpPort);
            TcpPort.Subscribe(value => inProjectConfig.Port = int.Parse(value));

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            inEventAggregator
                .GetEvent<ApplicationExitEvent>()
                .Subscribe(OnSaveConfig);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            if (inParam.ProjectConfig != null)
            {
                TcpPort.Value = inParam.ProjectConfig.Port.ToString();
            }
        }

        private void OnSaveConfig()
        {
            var projectConfigrepository = (Application.Current as PrismApplication).Container.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = projectConfigrepository.Load();

            projectConfig.Port = int.Parse(TcpPort.Value);
        }
    }
}
