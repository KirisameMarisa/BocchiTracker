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

        private UserConfig _userConfig;

        public void Initialize(CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepositoryFactory, ProjectConfig inProjectConfig)
        {
            Debug.Assert(inUserConfigRepository.Load() != null);

            _userConfig = inUserConfigRepository.Load();
            EnableOpenWebBrowser.Value = _userConfig.IsOpenWebBrowser;
        }

        public void Save(ref bool outIsNeedRestart)
        {
            _userConfig.IsOpenWebBrowser = EnableOpenWebBrowser.Value;
            outIsNeedRestart |= false;
        }
    }
}
