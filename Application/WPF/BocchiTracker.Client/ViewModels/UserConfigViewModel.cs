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
using Prism.Events;
using BocchiTracker.ModelEvent;
using System.Windows.Input;
using BocchiTracker.Client.Share.Commands;
using Prism.Commands;

namespace BocchiTracker.Client.ViewModels
{
    public class UserConfigViewModel : BindableBase
    {
        public ICommand SaveCommand { get; private set; }

        public UserConfigParts.AuthenticationParts AuthenticationParts { get; set; }
        public UserConfigParts.ChoiceProjectConfigParts ChoiceProjectConfigParts  { get; set; }
        public UserConfigParts.MiscParts MiscParts { get; set; }
        public UserConfigParts.MovieCaptureParts MovieCaptureParts { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private IAuthConfigRepositoryFactory _authConfigRepository;
        private CachedConfigRepository<ProjectConfig> _projectConfigRepository;
        private CachedConfigRepository<UserConfig> _userConfigRepository;

        public UserConfigViewModel(
            IEventAggregator inEventAggregator,
            CachedConfigRepository<ProjectConfig> inProjectConfigRepository, 
            CachedConfigRepository<UserConfig> inUserConfigRepository, 
            IAuthConfigRepositoryFactory inAuthConfigRepository)
        {
            SaveCommand = new DelegateCommand(OnSave);

            _eventAggregator = inEventAggregator;
            _authConfigRepository = inAuthConfigRepository;
            _userConfigRepository = inUserConfigRepository;
            _projectConfigRepository = inProjectConfigRepository;

            _eventAggregator
                .GetEvent<ConfigReloadEvent>()
                .Subscribe(OnConfigReload, ThreadOption.UIThread);

            AuthenticationParts = new UserConfigParts.AuthenticationParts();
            MiscParts = new UserConfigParts.MiscParts();
            ChoiceProjectConfigParts = new UserConfigParts.ChoiceProjectConfigParts();
            MovieCaptureParts = new UserConfigParts.MovieCaptureParts();
        }

        private void OnConfigReload(ConfigReloadEventParameter inParam)
        {
            var userConfig = _userConfigRepository.Load();
            if (userConfig == null)
                _userConfigRepository.Save(new UserConfig());

            foreach (var ui in new List<UserConfigParts.IConfig> { AuthenticationParts, ChoiceProjectConfigParts, MiscParts, MovieCaptureParts })
                ui.Initialize(_userConfigRepository, _authConfigRepository, _projectConfigRepository.Load());
        }

       public void OnSave()
       {
            bool isNeedRestart = false;
            foreach (var ui in new List<UserConfigParts.IConfig> { AuthenticationParts, ChoiceProjectConfigParts, MiscParts, MovieCaptureParts })
                ui.Save(ref isNeedRestart);

            var config = _userConfigRepository.Load();
            _userConfigRepository.Save(config);

            if (isNeedRestart) 
                MessageBox.Show("Save successed! Should restart client!", "BocchiTracker");
            else
                MessageBox.Show("Save successed!", "BocchiTracker");
       }
    }
}
