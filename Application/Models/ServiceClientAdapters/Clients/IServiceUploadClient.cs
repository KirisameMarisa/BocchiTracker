using BocchiTracker.ServiceClientData.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    public interface IServiceUploadClient : IService
    {
        bool IsAvailableFileUpload();

        Task<bool> UploadFiles(string inIssueKey, List<string> inFilenames);
    }
}
