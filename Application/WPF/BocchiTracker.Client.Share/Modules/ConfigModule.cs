using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Reflection;
using System.IO.Abstractions;
using System.IO;

namespace BocchiTracker.Client.Share.Modules
{
    public class ConfigModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var userConfigRepo = new ConfigRepository<UserConfig>(new FileSystem());
            userConfigRepo.SetLoadFilename(GetUserConfigFilePath());
            containerRegistry.RegisterInstance(new CachedConfigRepository<UserConfig>(userConfigRepo));
            containerRegistry.RegisterInstance(new CachedConfigRepository<ProjectConfig>(new ConfigRepository<ProjectConfig>(new FileSystem())));
        }

        private string GetUserConfigFilePath()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf('.'));
            var configFileName = $"{assemblyName}.{nameof(UserConfig)}.yaml";
            return Path.Combine("Configs", nameof(UserConfig) + "s", configFileName);
        }
    }
}
