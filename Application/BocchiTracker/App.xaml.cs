using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using BocchiTracker;
using BocchiTracker.Views;
using Prism.Regions;
using Slack.NetStandard.Objects;
using BocchiTracker.ViewModels;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.CrossServiceReporter;
using BocchiTracker.CrossServiceReporter.Converter;
using System.IO.Abstractions;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.IssueAssetCollector;
using BocchiTracker.ApplicationInfoCollector;
using System.Threading.Tasks;

namespace GameIssueTracker.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var issue_info_bundle = Container.Resolve<IssueInfoBundle>();
            Task.Run(() => issue_info_bundle.Initialize(Container.Resolve<IDataRepository>()));
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IServiceIssueClientFactory, ServiceIssueClientAdapterFactory>();
            containerRegistry.RegisterSingleton<IServiceUploadClientFactory, ServiceUploadClientAdapterFactory>();
            containerRegistry.RegisterSingleton<IDataRepository, DataRepository>();
            containerRegistry.RegisterSingleton<ICacheProvider, CacheProvider>();
            containerRegistry.RegisterSingleton<ITicketDataFactory, TicketDataFactory>();
            containerRegistry.RegisterSingleton<IIssuePoster, IssuePoster>();
            containerRegistry.RegisterSingleton<IAppInfoToCustomFieldsConverter, AppInfoToCustomFieldsConverter>();
            containerRegistry.RegisterSingleton<IAuthConfigRepositoryFactory, AuthConfigRepositoryFactory>();
            containerRegistry.Register<IPasswordService, PasswordService>();
            containerRegistry.Register<IFileSystem, FileSystem>();
            containerRegistry.RegisterSingleton(typeof(ConfigRepository<>));
            containerRegistry.RegisterSingleton(typeof(IssueInfoBundle));
            containerRegistry.RegisterSingleton(typeof(TrackerApplication));
            containerRegistry.RegisterSingleton(typeof(IssueAssetsBundle));
            containerRegistry.Register<IFilenameGenerator, TimestampedFilenameGenerator>();
        }
    }
}
