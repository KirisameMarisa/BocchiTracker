using System.Collections.Generic;
using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientAdapters.Clients.IssueClients;
using BocchiTracker.ServiceClientAdapters.Clients.UploadClients;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IServiceClientFactory
    {
        IService              CreateService(ServiceDefinitions serviceType);
        IServiceIssueClient?  CreateIssueService(ServiceDefinitions serviceType);
        IServiceUploadClient? CreateUploadService(ServiceDefinitions serviceType);
    }
   
    public class ServiceClientAdapterFactory : IServiceClientFactory
    {
        private static Dictionary<ServiceDefinitions, IService> _services = new Dictionary<ServiceDefinitions, IService>()
        {
            { ServiceDefinitions.JIRA,      new JIRAClient()          },
            { ServiceDefinitions.Redmine,   new RedmineClient()       },
            { ServiceDefinitions.Slack,     new SlackClient()         },
            { ServiceDefinitions.Github,    new GithubClient()        },
            { ServiceDefinitions.Glitlab,   new GitlabClient()        },
            { ServiceDefinitions.Discord,   new DiscordClient()       },
            { ServiceDefinitions.Explorer,  new ExplorerClients()     },
            { ServiceDefinitions.Dropbox,   new DropboxClients()      },
        };

        public IService CreateService(ServiceDefinitions serviceType)
        {
            return _services[serviceType];
        }

        public IServiceIssueClient? CreateIssueService(ServiceDefinitions serviceType)
        {
            return _services[serviceType] as IServiceIssueClient;
        }

        public IServiceUploadClient? CreateUploadService(ServiceDefinitions serviceType)
        {
            var service = _services[serviceType] as IServiceUploadClient;
            return service != null && service.IsAvailableFileUpload() 
                ? service
                : null;
        }
    }
}
