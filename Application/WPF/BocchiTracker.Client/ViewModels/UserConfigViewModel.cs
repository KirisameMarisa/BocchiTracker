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
using BocchiTracker.Client.ViewModels.UserConfigParts;
using System.Windows;

namespace BocchiTracker.Client.ViewModels
{
    public class UserConfigViewModel : BindableBase, IDialogAware
    {
        public string Title => "User Config";
        public event Action<IDialogResult> RequestClose;

        public UserConfigParts.AuthenticationParts AuthenticationParts { get; set; }
        public UserConfigParts.ChoiceProjectConfigParts ChoiceProjectConfigParts  { get; set; }
        public UserConfigParts.MiscParts MiscParts { get; set; }

        private IAuthConfigRepositoryFactory _authConfigRepository;
        private CachedConfigRepository<ProjectConfig> _projectConfigRepository;
        private CachedConfigRepository<UserConfig> _userConfigRepository;

        public UserConfigViewModel(CachedConfigRepository<ProjectConfig> inProjectConfigRepository, CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepository)
        {
            _authConfigRepository = inAuthConfigRepository;
            _userConfigRepository = inUserConfigRepository;
            _projectConfigRepository = inProjectConfigRepository;

            var userConfig = _userConfigRepository.Load();
            if (userConfig == null)
                inUserConfigRepository.Save(new UserConfig());

            AuthenticationParts = new UserConfigParts.AuthenticationParts(_authConfigRepository, _projectConfigRepository.Load());
            MiscParts = new UserConfigParts.MiscParts(inUserConfigRepository);
            ChoiceProjectConfigParts = new UserConfigParts.ChoiceProjectConfigParts(inUserConfigRepository);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            bool isNeedRestart = false;

            AuthenticationParts.Save(ref isNeedRestart);
            ChoiceProjectConfigParts.Save(ref isNeedRestart);
            MiscParts.Save(ref isNeedRestart);

            var config = _userConfigRepository.Load();
            _userConfigRepository.Save(config);

            if (isNeedRestart) 
                MessageBox.Show("Should restart client!", "BocchiTracker");

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
