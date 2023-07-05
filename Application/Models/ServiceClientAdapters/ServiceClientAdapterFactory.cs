using System;
using System.Collections.Generic;
using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters.IssueClients;
using BocchiTracker.ServiceClientAdapters.UploadClients;
using Octokit;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IServiceIssueClientFactory
    {
        IServiceIssueClient CreateServiceClientAdapter(IssueServiceDefinitions serviceType);
    }

    public interface IServiceUploadClientFactory
    {
        IServiceUploadClient CreateServiceClientAdapter(UploadServiceDefinitions serviceType);
    }

    public class ServiceIssueClientAdapterFactory : IServiceIssueClientFactory
    {
        private static Dictionary<IssueServiceDefinitions, IServiceIssueClient> _services = new Dictionary<IssueServiceDefinitions, IServiceIssueClient>()
        {
            { IssueServiceDefinitions.JIRA,      new JIRAClient()    },
            { IssueServiceDefinitions.Redmine,   new RedmineClient() },
            { IssueServiceDefinitions.Slack,     new SlackClient()   },
            { IssueServiceDefinitions.Github,    new GithubClient()  },
            { IssueServiceDefinitions.Glitlab,   new GitlabClient()  },
            { IssueServiceDefinitions.Discord,   new DiscordClient() },

        };

        public IServiceIssueClient CreateServiceClientAdapter(IssueServiceDefinitions serviceType)
        {
            return _services[serviceType];
        }
    }

    public class ServiceUploadClientAdapterFactory : IServiceUploadClientFactory
    {
        private static Dictionary<UploadServiceDefinitions, IServiceUploadClient> _services = new Dictionary<UploadServiceDefinitions, IServiceUploadClient>()
        {
            { UploadServiceDefinitions.JIRA,      new JIRAClient()    },
            { UploadServiceDefinitions.Redmine,   new RedmineClient() },
            { UploadServiceDefinitions.Slack,     new SlackClient()   },
            { UploadServiceDefinitions.Github,    new GithubClient()  },
            { UploadServiceDefinitions.Glitlab,   new GitlabClient()  },
            { UploadServiceDefinitions.Discord,   new DiscordClient() },
            { UploadServiceDefinitions.Explorer,  new ExplorerClients()    },
            { UploadServiceDefinitions.Dropbox,   new DropboxClients() },
        };

        public IServiceUploadClient CreateServiceClientAdapter(UploadServiceDefinitions serviceType)
        {
            return _services[serviceType];
        }
    }
}
