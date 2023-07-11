using BocchiTracker.ModelEventBus;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector.Handlers
{
    public class AppDisconnectHandler
    {
        private AppStatusBundles _bundles;

        public AppDisconnectHandler(IEventAggregator inEventAggregator, AppStatusBundles inBundles)
        {
            _bundles = inBundles;

            inEventAggregator
                .GetEvent<AppDisconnectEvent>()
                .Subscribe(Handle, ThreadOption.BackgroundThread);
        }

        public void Handle(AppDisconnectEventParameter inParameter)
        {
            _bundles.Remove(inParameter.ClientID);
        }
    }
}
