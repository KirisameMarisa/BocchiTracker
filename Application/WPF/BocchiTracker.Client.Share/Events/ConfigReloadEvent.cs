using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Client.Share.Events
{
    public class ConfigReloadEventParameter
    {
        public ProjectConfig ProjectConfig { get; set; }

        public UserConfig UserConfig { get; set; }

        public Dictionary<ServiceDefinitions, AuthConfig> AuthConfigs { get; set; } = new Dictionary<ServiceDefinitions, AuthConfig>();

        public ConfigReloadEventParameter(ProjectConfig inProjectConfig, UserConfig userConfig)
        {
            ProjectConfig = inProjectConfig;
            UserConfig = userConfig;
        }
    }

    public class ConfigReloadEvent : PubSubEvent<ConfigReloadEventParameter> {}
}
