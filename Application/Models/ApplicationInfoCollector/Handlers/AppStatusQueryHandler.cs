using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ModelEventBus;
using System.Threading;
using BocchiTracker.ProcessLinkQuery.Queries;

namespace BocchiTracker.ApplicationInfoCollector.Handlers
{
    public class AppStatusQueryHandler : IRequestHandler<AppStatusQueryEvent>
    {
        private AppStatusBundles _bundles;

        public AppStatusQueryHandler(AppStatusBundles inBundles)
        {
            _bundles = inBundles;
        }

        public Task Handle(AppStatusQueryEvent request, CancellationToken cancellationToken)
        {
            switch (request.AppStatus.QueryID)
            {
                case (byte)QueryID.AppBasicInfo: //!< AppBaicInfo Magic number...
                    ProcessAppBasicInfo(request.AppStatus.ClientID, request.AppStatus.Status); break;

                default:
                    ProcessAppStatusDynamic(request.AppStatus.ClientID, request.AppStatus.Status); break;
            }
            return Task.CompletedTask;
        }

        private void ProcessAppBasicInfo(int inClientID, Dictionary<string, dynamic>? inData)
        {
            _bundles.Add(inClientID);
            
            if (inData is not null)
            {
                _bundles[inClientID].AppBasicInfo.Pid = inData["Pid"] ?? "";
                _bundles[inClientID].AppBasicInfo.AppName = inData["AppName"] ?? "";
                _bundles[inClientID].AppBasicInfo.Args = inData["Args"] ?? "";
                _bundles[inClientID].AppBasicInfo.Platform = inData["Platform"] ?? "";
            }
        }

        private void ProcessAppStatusDynamic(int inClientID, Dictionary<string, dynamic>? inData)
        {
            _bundles.Add(inClientID);

            if (inData is not null)
            {
                foreach(var item in inData)
                {
                    _bundles[inClientID].AppStatusDynamics[item.Key] = item.Value;
                }
            }
        }
    }
}
