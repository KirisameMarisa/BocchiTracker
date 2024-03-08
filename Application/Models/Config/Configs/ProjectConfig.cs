using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BocchiTracker.Config.Parts;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.Config.Configs
{
    public class CaptureSetting
    {
        public string? FFmpegPath { get; set; }

        public SIPSorceryMedia.Abstractions.VideoCodecsEnum VideoCodecs { get; set; } = SIPSorceryMedia.Abstractions.VideoCodecsEnum.VP8;
    }

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

        public List<ValueMapping>               PriorityMappings { get; set; } = new List<ValueMapping>();

        public List<ValueMapping>               TicketTypeMappings { get; set; } = new List<ValueMapping>();

        public List<ValueMapping>               IssueGradeMappings { get; set; } = new List<ValueMapping>();

        public List<ValueMapping>               QueryFieldMappings { get; set; } = new List<ValueMapping>();

        public List<ValueMapping>               DefaultValue { get; set;} = new List<ValueMapping>();
    }

    public class ProjectConfig
    {
        public int Port { get; set; }                   = 8888;

        public int WebSocketPort { get; set; }          = 8822;

        public List<string> TicketTypes { get; set; }  = new List<string> { "Bug", "Task", "Question" };

        public List<string> Priorities { get; set; }   = new List<string> { "Low", "Middle", "High" };

        public List<string> IssueGrades { get; set; }  = new List<string> { "A", "B", "C" };

        public List<string> QueryFields { get; set; } = new List<string>();

        public string FileSaveDirectory { get; set; }  = Path.Combine(Environment.CurrentDirectory, "FileSave");

        public CaptureSetting CaptureSetting { get; set; } = new CaptureSetting();

        public List<MonitoredDirectoryConfig> MonitoredDirectoryConfigs { get; set; } = new List<MonitoredDirectoryConfig>();

        public string? CacheDirectory { get; set; }

        public ExternalToolsPath ExternalToolsPath { get; set; } = new ExternalToolsPath();

        public List<ServiceConfig> ServiceConfigs { get; set; } = new List<ServiceConfig> 
        {
            new ServiceConfig { Service = ServiceDefinitions.Redmine    },
            new ServiceConfig { Service = ServiceDefinitions.Github     },
            new ServiceConfig { Service = ServiceDefinitions.Slack      },
        };

        public ServiceConfig? GetServiceConfig(ServiceDefinitions inServiceDefinitions)
        {
            return ServiceConfigs.Find(x => x.Service == inServiceDefinitions);
        }
    }
}