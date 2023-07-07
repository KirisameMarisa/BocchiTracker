using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateTicketType : ICreateUnifiedTicketData<string>
    {
        public string? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.TicketTypeMappings == null || inBundle.TicketData.TicketType == null)
                return null;

            if (!inConfig.TicketTypeMappings.ContainsKey(inBundle.TicketData.TicketType))
                return null;

            return inConfig.TicketTypeMappings[inBundle.TicketData.TicketType];
        }
    }
}
