using System.Collections.Generic;

namespace BocchiTracker.Config.Configs
{
    public class MonitoredDirectoryConfig
    {
        public string? Directory { get; set; }

        public string Filter { get; set; } = string.Empty;
    }

    public class ServiceConfig
    {
        public ServiceDefinitions               Service { get; set; }

        public string?                          URL { get; set; }

        public string?                          ProxyURL { get; set; }

        public string?                          DescriptionFormat { get; set; }

        public Dictionary<string, string>?      PriorityMappings { get; set; }

        public Dictionary<string, string>?      TicketTypeMappings { get; set; }

        public Dictionary<string, string>?      QueryFieldMappings { get; set; }

        public Dictionary<string, string>?      DefaultValue { get; set;}
    }

    public class ProjectConfig
    {
        public List<string>? TicketTypes { get; set; }

        public List<string>? Priorities { get; set; }

        public List<string>? Classes { get; set; }

        public List<MonitoredDirectoryConfig>? MonitoredDirectoryConfigs { get; set; }

        public string? CacheDirectory { get; set; }

        public List<ServiceConfig>? ServiceConfigs { get; set; }

        public ServiceConfig? GetServiceConfig(ServiceDefinitions inServiceDefinitions)
        {
            if (ServiceConfigs == null)
                return null;
            return ServiceConfigs.Find(x => x.Service == inServiceDefinitions);
        }
    }
}