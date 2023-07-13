using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BocchiTracker.Config.Configs
{
    public class MonitoredDirectoryConfig
    {
        public string? Directory { get; set; }

        public string Filter { get; set; } = string.Empty;
    }

    public class ValueMapping
    {
        public string Definition { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }

    public class ServiceConfig
    {
        public ServiceDefinitions               Service { get; set; }

        public string?                          URL { get; set; }

        public string?                          ProxyURL { get; set; }

        public string?                          DescriptionFormat { get; set; }

        public List<ValueMapping>?              PriorityMappings { get; set; }

        public List<ValueMapping>?              TicketTypeMappings { get; set; }

        public List<ValueMapping>?              QueryFieldMappings { get; set; }

        public List<ValueMapping>?              DefaultValue { get; set;}
    }

    public class ExternalToolsPath
    {
        public string? ProcDumpPath { get; set; }
    }

    public class ProjectConfig
    {
        public int Port { get; set; } = 8888;

        public List<string>? TicketTypes { get; set; }

        public List<string>? Priorities { get; set; }

        public List<string>? Classes { get; set; }

        public string FileSaveDirectory { get; set; } = Path.Combine(Environment.CurrentDirectory, "FileSave");

        public List<MonitoredDirectoryConfig>? MonitoredDirectoryConfigs { get; set; }

        public string? CacheDirectory { get; set; }

        public ExternalToolsPath ExternalToolsPath { get; set; } = new ExternalToolsPath();

        public List<ServiceConfig>? ServiceConfigs { get; set; }

        public ServiceConfig? GetServiceConfig(ServiceDefinitions inServiceDefinitions)
        {
            if (ServiceConfigs == null)
                return null;
            return ServiceConfigs.Find(x => x.Service == inServiceDefinitions);
        }
    }
}