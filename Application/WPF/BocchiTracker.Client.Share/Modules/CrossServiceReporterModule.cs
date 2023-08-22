using BocchiTracker.CrossServiceReporter.Converter;
using BocchiTracker.CrossServiceReporter;
using Prism.Ioc;
using Prism.Modularity;

namespace BocchiTracker.Client.Share.Modules
{
    public class CrossServiceReporterModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfoToCustomFieldsConverter, AppInfoToCustomFieldsConverter>();
            containerRegistry.RegisterSingleton<ITicketDataFactory, TicketDataFactory>();
            containerRegistry.RegisterSingleton<IIssuePoster, IssuePoster>();
            containerRegistry.RegisterSingleton<IIssueOpener, IssueOpener>();
        }
    }
}
