using BocchiTracker.ProcessLink.CreateRequest;
using BocchiTracker.ProcessLink.ProcessData;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.ProcessLink
{
    public class ProcessLinkModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var serviceProcessData = containerProvider.Resolve<IServiceProcessData>();
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.AppBasicInfo, new ProcessDataAppBasicInfo());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.PlayerPosition, new ProcessDataPlayerPosition());
            serviceProcessData.Register(ProcessLinkQuery.Queries.QueryID.ScreenshotData, new ProcessDataScreenshotData());

            var serviceCreateRequest = containerProvider.Resolve<IServiceCreateRequest>();
            serviceCreateRequest.Register(ProcessLinkQuery.Queries.QueryID.ScreenshotRequest, new CreateRequestScreenshot());
            serviceCreateRequest.Register(ProcessLinkQuery.Queries.QueryID.JumpRequest, new CreateRequestJump());
            serviceCreateRequest.Register(ProcessLinkQuery.Queries.QueryID.IssueesRequest, new CreateRequestIssues());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IServiceProcessData, ServiceProcessData>();
            containerRegistry.RegisterSingleton<IServiceCreateRequest, ServiceCreateRequest>();
            containerRegistry.RegisterSingleton(typeof(Connection));
        }
    }
}
