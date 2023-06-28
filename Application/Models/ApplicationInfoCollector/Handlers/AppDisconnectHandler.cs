using BocchiTracker.ModelEventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector.Handlers
{
    public class AppDisconnectHandler : IRequestHandler<AppDisconnectEvent>
    {
        private AppStatusBundles _bundles;

        public AppDisconnectHandler(AppStatusBundles inBundles)
        {
            _bundles = inBundles;
        }

        public Task Handle(AppDisconnectEvent request, CancellationToken cancellationToken)
        {
            _bundles.Remove(request.ClientID);
            return Task.CompletedTask;
        }
    }
}
