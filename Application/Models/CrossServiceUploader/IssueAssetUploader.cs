using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.IssueClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceUploader
{
    public interface IIssueAssetUploader
    {
        Task Upload(IssueServiceDefinitions inService, string inIssueKey, IssueAssetsBundle inBundle, ProjectConfig inConfig);
    }

    public class IssueAssetUploader : IIssueAssetUploader
    {
        private readonly IServiceIssueClientFactory _client_factory;

        public IssueAssetUploader(IServiceIssueClientFactory inClientFactory)
        {
            _client_factory = inClientFactory;
        }

        public Task Upload(IssueServiceDefinitions inService, string inIssueKey, IssueAssetsBundle inBundle, ProjectConfig inConfig)
        {
            var client = _client_factory.CreateServiceClientAdapter(inService);


            throw new NotImplementedException();
        }
    }
}
