using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ServiceClientAdapters;
using System;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceUploader
{
    public interface IIssueAssetUploader
    {
        Task Upload(ServiceDefinitions inService, string inIssueKey, IssueAssetsBundle inBundle, ProjectConfig inConfig);
    }

    public class IssueAssetUploader : IIssueAssetUploader
    {
        private readonly IServiceClientFactory _client_factory;

        public IssueAssetUploader(IServiceClientFactory inClientFactory)
        {
            _client_factory = inClientFactory;
        }

        public Task Upload(ServiceDefinitions inService, string inIssueKey, IssueAssetsBundle inBundle, ProjectConfig inConfig)
        {
            var client = _client_factory.CreateUploadService(inService);


            throw new NotImplementedException();
        }
    }
}
