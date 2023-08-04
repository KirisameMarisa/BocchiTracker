using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Client.Config.Controls
{
    public class ServiceValueMapping
    {
        public ReactiveProperty<string> Definition  { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Redmine     { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Slack       { get; set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Github      { get; set; } = new ReactiveProperty<string>();

        public ServiceValueMapping(string inDefinition) 
        {
            Definition.Value = inDefinition;
        }

        public void SetServiceName(ServiceDefinitions inServiceName, string inName)
        {
            switch (inServiceName)
            {
                case ServiceDefinitions.Redmine:
                    Redmine.Value   = inName; break;
                case ServiceDefinitions.Slack:
                    Slack.Value     = inName; break;
                case ServiceDefinitions.Github:
                    Github.Value    = inName; break;
                default:
                    break;
            }
        }

        public string GetServiceName(ServiceDefinitions inService)
        {
            switch (inService)
            {
                case ServiceDefinitions.Redmine:
                    return Redmine.Value;
                case ServiceDefinitions.Slack:
                    return Slack.Value;
                case ServiceDefinitions.Github:
                    return Github.Value;
                default:
                    return string.Empty;
            }
        }
    }
}
