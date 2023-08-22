using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters;
using Reactive.Bindings;
using BocchiTracker.Config;

namespace BocchiTracker.Client.ViewModels
{
    internal class UserConfigViewModel : BindableBase, IDialogAware
    {
        public ReactiveProperty<bool> EnableOpenWebBrowser { get; set; } = new ReactiveProperty<bool>();

        public string Title => "User Config";
        public event Action<IDialogResult> RequestClose;

        private CachedConfigRepository<UserConfig> _userConfigRepository;
        private IAuthConfigRepositoryFactory _authConfigRepository;

        public UserConfigViewModel(CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepository)
        {
            _userConfigRepository = inUserConfigRepository;
            _authConfigRepository = inAuthConfigRepository;

            var userConfig = _userConfigRepository.Load();
            if(userConfig != null)
            {
                EnableOpenWebBrowser.Value = userConfig.IsOpenWebBrowser;
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            var userConfig = _userConfigRepository.Load() ?? new UserConfig();
            userConfig.IsOpenWebBrowser = EnableOpenWebBrowser.Value;
            _userConfigRepository.Save(userConfig);
            
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
