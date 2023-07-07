using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.CrossServiceReporter.Converter;

namespace BocchiTracker.CrossServiceReporter
{
    public interface ITicketDataFactory
    {
        TicketData? Create(ServiceDefinitions inService, IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig);
    }

    public class TicketDataFactory : ITicketDataFactory
    {
        IAppInfoToCustomFieldsConverter _converter;

        public TicketDataFactory(IAppInfoToCustomFieldsConverter inConverter)
        {
            _converter = inConverter;
        }

        public TicketData? Create(ServiceDefinitions inService, IssueInfoBundle inIssueBundle, AppStatusBundle inAppBundle, ProjectConfig inConfig)
        {
            var config = inConfig.GetServiceConfig(inService);
            if (config == null)
                return null;

            inIssueBundle.TicketData.CustomFields = _converter.Convert(inAppBundle);

            return new CreateTicketData.CreateTicketData().Create(inService, inIssueBundle, config);
        }
    }
}
