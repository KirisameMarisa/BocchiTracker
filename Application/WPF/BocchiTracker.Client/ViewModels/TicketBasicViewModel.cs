using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.Data;
using BocchiTracker.Client.Share.Events;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.IssueInfoCollector;
using Prism.Events;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Prism.Commands;
using System.Windows.Input;
using System.Windows;
using Slack.NetStandard.Messages.Blocks;
using System.Diagnostics;
using System.IO;

namespace BocchiTracker.Client.ViewModels
{
    public class TicketBasicViewModel : BindableBase
    {
        public TicketProperty TicketProperty { get; set; }
        public ReactiveCollection<string> TicketTypes { get; }
        public ICommand RunConfigCommand { get; }

        private string useProjectConfig = default!;

        public TicketBasicViewModel(IEventAggregator inEventAggregator, TicketProperty inTicketProperty)
        {
            TicketProperty = inTicketProperty;
            TicketTypes = new ReactiveCollection<string>();
            RunConfigCommand = new DelegateCommand(OnRunConfig);

            inEventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            foreach (var item in inParam.ProjectConfig.TicketTypes)
                TicketTypes.Add(item);

            if(inParam.UserConfig != null)
            {
                useProjectConfig = inParam.UserConfig.ProjectConfigFilename;
            }
        }

        private void OnRunConfig()
        {
            using (Process proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = "BocchiTracker.Client.Config.exe",
                    UseShellExecute = false,
                    Arguments = $"{useProjectConfig} /r"
                };
                proc.Start();
                proc.WaitForExit();
            }
        }
    }
}
