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
        public string? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.PriorityMappings == null || inBundle.TicketData.Priority == null)
                return null;

            var find = inConfig.PriorityMappings.Find(x => x.Definition == inBundle.TicketData.Priority);
            if (find == null)
                return null;

            return find.Name;
        }
    }
}
