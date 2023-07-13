using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Reflection;
using System.IO.Abstractions;
using System.IO;

namespace BocchiTracker.Modules
{
    public class ConfigModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) 
        {
            var configRepo = containerProvider.Resolve<CachedConfigRepository<ProjectConfig>>();
            var projectConfig = configRepo.Load();
            //!< force exit?
            if (projectConfig == null)
                return;

            if (!CreateWorkingDirectory(projectConfig))
                return;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(new CachedConfigRepository<UserConfig>(
                new ConfigRepository<UserConfig>(GetUserConfigFilePath(), new FileSystem())));

            containerRegistry.RegisterInstance(new CachedConfigRepository<ProjectConfig>(
                new ConfigRepository<ProjectConfig>(GetProjectConfigFilePath(), new FileSystem())));
        }

        private string GetProjectConfigFilePath()
        {
            string projectConfigName = null;
            var args = Environment.GetCommandLineArgs();
            if (args.Length <= 2)
                projectConfigName = args[1];

            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFileName = $"{projectConfigName ?? assemblyName}.{nameof(ProjectConfig)}.yaml";
            return Path.Combine("Configs", nameof(ProjectConfig) + "s", configFileName);
        }

        private string GetUserConfigFilePath()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var configFileName = $"{assemblyName}.{nameof(UserConfig)}.yaml";
            return Path.Combine("Configs", nameof(UserConfig) + "s", configFileName);
        }

        public bool CreateWorkingDirectory(ProjectConfig inProjectConfig)
        {
            try
            {
                Directory.CreateDirectory(inProjectConfig.FileSaveDirectory);
                return Directory.Exists(inProjectConfig.FileSaveDirectory);
            }
            catch { return false; }
        }
    }
}
