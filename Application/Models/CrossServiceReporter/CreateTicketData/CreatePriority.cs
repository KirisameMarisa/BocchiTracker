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
    public class CreatePriority : ICreateUnifiedTicketData<string>
    {
        public string? Create(IssueServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.PriorityMappings == null || inBundle.TicketData.Priority == null)
                return null;

            if (!inConfig.PriorityMappings.ContainsKey(inBundle.TicketData.Priority))
                return null;

            return inConfig.PriorityMappings[inBundle.TicketData.Priority];
        }
    }
}
