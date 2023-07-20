using BocchiTracker.Client.Config.Views;
using BocchiTracker.Client.Share.Modules;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.IO.Abstractions;
using System.Windows;

namespace BocchiTracker.Client.Config
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileSystem, FileSystem>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ConfigModule>();
            moduleCatalog.AddModule<ServiceClientAdaptersModule>(
                dependsOn: new string[]
                {
                                typeof(ConfigModule).Name
                });
        }
    }
}
