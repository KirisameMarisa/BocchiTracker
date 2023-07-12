using BocchiTracker.ApplicationInfoCollector;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BocchiTracker.ViewModels
{
    public class ConnectedToViewModel : BindableBase
    {
        private AppStatusBundles _appStatusBundles;

        public ObservableCollection<AppStatusBundle> Items { get; } = new ObservableCollection<AppStatusBundle>();

        public AppStatusBundle Selected 
        {
            get { return _appStatusBundles.TrackerApplication; }
            set { SetProperty(ref _appStatusBundles.TrackerApplication, value);  }
        }

        public ConnectedToViewModel(AppStatusBundles inAppStatusBundles)
        {
            _appStatusBundles = inAppStatusBundles;
            _appStatusBundles.AppConnected      = this.Connected;
            _appStatusBundles.AppDisconnected   = this.Disconnected;
            Initialize();
        }

        public void Initialize()
        {
            foreach (var appStatusBundle in _appStatusBundles.Bundles.Values)
            {
                Items.Add(appStatusBundle);
            }
        }

        public void Connected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle);
                if (foundItems.Count() == 0)
                {
                    Items.Add(inAppStatusBundle);
                }
            });
        }

        public void Disconnected(AppStatusBundle inAppStatusBundle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var foundItems = Items.Where(x => x == inAppStatusBundle);
                foreach (var removeItem in foundItems)
                {
                    Items.Remove(removeItem);
                }
            });
        }
    }
}
