using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientAdapters;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Client.ViewModels.UserConfigParts
{
    public class MiscParts : BindableBase, IConfig
    {
        public ReactiveProperty<bool> EnableOpenWebBrowser { get; set; } = new ReactiveProperty<bool>();

        private CachedConfigRepository<UserConfig> _userConfigRepository;

        public MiscParts(CachedConfigRepository<UserConfig> inUserConfigRepository)
        {
            _userConfigRepository = inUserConfigRepository;
            Debug.Assert(_userConfigRepository.Load() != null);

            EnableOpenWebBrowser.Value = _userConfigRepository.Load().IsOpenWebBrowser;
        }

        public void Save()
        {
            var userConfig = _userConfigRepository.Load();
            userConfig.IsOpenWebBrowser = EnableOpenWebBrowser.Value;
            _userConfigRepository.Save(userConfig);
        }
    }
}
