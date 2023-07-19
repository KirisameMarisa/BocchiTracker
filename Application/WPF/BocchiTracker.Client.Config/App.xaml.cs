using BocchiTracker.Client.Config.Views;
using Prism.Ioc;
using Prism.Unity;
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
        }
    }
}
