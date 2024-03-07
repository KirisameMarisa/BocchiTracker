using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace BocchiTracker.Client.ViewModels.UserConfigParts
{
    public class MovieCaptureParts : BindableBase, IConfig
    {
        public ReactiveProperty<bool> NotUse { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> UseWebRTC { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> UseOBS { get; set; } = new ReactiveProperty<bool>(false);

        private UserConfig _userConfig;

        public void Initialize(CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepositoryFactory, ProjectConfig inProjectConfig)
        {
            Debug.Assert(inUserConfigRepository.Load() != null);

            _userConfig = inUserConfigRepository.Load();
            switch(_userConfig.CaptureSetting.GameCaptureType)
            {
                case GameCaptureType.NotUse: NotUse.Value = true; break;
                case GameCaptureType.WebRTC: UseWebRTC.Value = true; break;
                case GameCaptureType.OBSStudio: UseOBS.Value = true; break;
            }
        }

        public void Save(ref bool outIsNeedRestart)
        {
            outIsNeedRestart |= true;

            if (NotUse.Value)
            {
                _userConfig.CaptureSetting.GameCaptureType = GameCaptureType.NotUse;
                return;
            }
            if (UseWebRTC.Value)
            {
                _userConfig.CaptureSetting.GameCaptureType = GameCaptureType.WebRTC;
                return;
            }
            if (UseOBS.Value)
            {
                _userConfig.CaptureSetting.GameCaptureType = GameCaptureType.OBSStudio;
                return;
            }
        }
    }
}
