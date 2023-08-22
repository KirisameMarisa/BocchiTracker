using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.CrossServiceUploader;

namespace BocchiTracker.CrossServiceUploader
{
    public class CrossServiceUploaderModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IIssueAssetUploader, IssueAssetUploader>();
        }
    }
}
