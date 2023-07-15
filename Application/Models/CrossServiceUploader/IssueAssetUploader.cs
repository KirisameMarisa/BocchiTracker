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
        private readonly IServiceClientFactory _clientFactory;

        public IssueAssetUploader(IServiceClientFactory inClientFactory)
        {
            _clientFactory = inClientFactory;
        }

        public async Task Upload(ServiceDefinitions inService, string inIssueKey, IssueAssetsBundle inBundle, ProjectConfig inConfig)
        {
            var client = _clientFactory.CreateUploadService(inService);
            if (client == null)
                return;

            var files = inBundle.GetFiles();
            if(files.Count == 0) 
                return;

            await client.UploadFiles(inIssueKey, files);
        }
    }
}
