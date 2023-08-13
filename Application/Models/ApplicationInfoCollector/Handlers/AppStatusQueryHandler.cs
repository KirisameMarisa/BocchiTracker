using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ModelEvent;
using System.Threading;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;

namespace BocchiTracker.ApplicationInfoCollector.Handlers
{
    public class AppStatusQueryHandler
    {
        private AppStatusBundles _bundles;

        public AppStatusQueryHandler(IEventAggregator inEventAggregator, AppStatusBundles inBundles)
        {
            _bundles = inBundles;

            inEventAggregator
                .GetEvent<AppStatusQueryEvent>()
                .Subscribe(Handle, ThreadOption.BackgroundThread);
        }

        public void Handle(AppStatusQueryEventParameter inParameter)
        {
            switch (inParameter.AppStatus.QueryID)
            {
                case (byte)QueryID.AppBasicInfo: //!< AppBaicInfo Magic number...
                    ProcessAppBasicInfo(inParameter.AppStatus.ClientID, inParameter.AppStatus.Status); break;

                default:
                    ProcessAppStatusDynamic(inParameter.AppStatus.ClientID, inParameter.AppStatus.Status); break;
            }
        }

        private void ProcessAppBasicInfo(int inClientID, Dictionary<string, string>? inData)
        {
            _bundles.Add(inClientID);
            
            if (inData is not null)
            {
                foreach(var (key, value) in inData)
                {
                    _bundles[inClientID].AppBasicInfo.Set(key, value);
                }
            }
        }

        private void ProcessAppStatusDynamic(int inClientID, Dictionary<string, string>? inData)
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
