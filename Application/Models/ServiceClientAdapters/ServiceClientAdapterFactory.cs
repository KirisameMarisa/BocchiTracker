using System;
using System.Collections.Generic;
using BocchiTracker.ProjectConfig;
using BocchiTracker.ServiceClientAdapters.Clients;
using Octokit;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IServiceClientAdapterFactory
    {
        IServiceClientAdapter CreateServiceClientAdapter(ServiceDefinitions serviceType);
    }

    public class ServiceClientAdapterFactory : IServiceClientAdapterFactory
    {
        private static Dictionary<ServiceDefinitions, IServiceClientAdapter> _services = new Dictionary<ServiceDefinitions, IServiceClientAdapter>()
        {
            { ServiceDefinitions.JIRA,     new JIRAClient() },
            { ServiceDefinitions.Redmine,  new RedmineClient() },
            { ServiceDefinitions.Slack,    new SlackClient() },
            { ServiceDefinitions.Github,   new GithubClient() },
            { ServiceDefinitions.Glitlab,   new GitlabClient() },
            { ServiceDefinitions.Discord,   new DiscordClient() },

        };

        public IServiceClientAdapter CreateServiceClientAdapter(ServiceDefinitions serviceType)
        {
            return _services[serviceType];
        }
    }
}
