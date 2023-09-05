using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Reactive.Bindings;
using System.IO;
using BocchiTracker.ServiceClientAdapters;

namespace BocchiTracker.Client.ViewModels.UserConfigParts
{
    public class ChoiceProjectConfigParts : BindableBase, IConfig
    {
        public string ProjectConfigDirectory;

        public ReactiveCollection<string> ProjectConfigs { get; set; } = new ReactiveCollection<string>();
        public ReactiveProperty<string> UseProjectConfig { get; set; } = new ReactiveProperty<string>();

        private UserConfig _userConfig;

        public void Initialize(CachedConfigRepository<UserConfig> inUserConfigRepository, IAuthConfigRepositoryFactory inAuthConfigRepositoryFactory, ProjectConfig inProjectConfig)
        {
            Debug.Assert(inUserConfigRepository.Load() != null);

            _userConfig = inUserConfigRepository.Load();

            ProjectConfigDirectory = Path.GetDirectoryName(_userConfig.ProjectConfigFilename);
            if (Directory.Exists(ProjectConfigDirectory))
            {
                foreach (var file in Directory.GetFiles(ProjectConfigDirectory, "*.yaml"))
                {
                    ProjectConfigs.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            UseProjectConfig.Value = Path.GetFileNameWithoutExtension(_userConfig.ProjectConfigFilename);
        }

        public void Save(ref bool outIsNeedRestart)
        {
            string filename = Path.Combine(ProjectConfigDirectory, UseProjectConfig.Value + ".yaml");
            if (string.IsNullOrEmpty(filename))
                return;
            if (!System.IO.File.Exists(filename))
                return;

            if (_userConfig.ProjectConfigFilename != filename)
                outIsNeedRestart |= true;
            _userConfig.ProjectConfigFilename = filename;
        }
    }
}
