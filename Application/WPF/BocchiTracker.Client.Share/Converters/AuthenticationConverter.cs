using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BocchiTracker.Client.Share.Converters
{
    public class AuthenticationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string serviceName)
            {
                if(Enum.TryParse<ServiceDefinitions>(serviceName, out var service))
                {
#if FALSE
                    var factory = (Application.Current as PrismApplication).Container.Resolve<ServiceClientAdapterFactory>();
                    var client = factory.CreateService(service);
                    return client != null && client.IsAuthenticated()
                        ? Visibility.Visible
                        : Visibility.Collapsed;
#else
                    return Visibility.Visible;
#endif
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
