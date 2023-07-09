using BocchiTracker.Config.Configs;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Event
{
    public class ConfigReloadEventParameter
    {
        public ProjectConfig ProjectConfig { get; set; }

        public ConfigReloadEventParameter(ProjectConfig inProjectConfig)
        {
            ProjectConfig = inProjectConfig;
        }
    }

    public class ConfigReloadEvent : PubSubEvent<ConfigReloadEventParameter> {}
}
