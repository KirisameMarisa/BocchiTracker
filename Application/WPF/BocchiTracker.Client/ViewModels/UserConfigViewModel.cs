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

namespace BocchiTracker.Client.ViewModels
{
    public class UserConfigViewModel : BindableBase, IDialogAware
    {
        public string Title => "User Config";
        public event Action<IDialogResult> RequestClose;

        public UserConfigParts.AuthenticationParts AuthenticationParts { get; set; }
        public UserConfigParts.MiscParts MiscParts { get; set; }

        public UserConfigViewModel(CachedConfigRepository<ProjectConfig> inProjectConfigRepository, CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepository)
        {
            AuthenticationParts = new UserConfigParts.AuthenticationParts(inAuthConfigRepository, inProjectConfigRepository);
            MiscParts = new UserConfigParts.MiscParts(inUserConfigRepository);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            AuthenticationParts.Save();
            MiscParts.Save();

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
