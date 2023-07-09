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

            var find = inConfig.TicketTypeMappings.Find(x => x.Definition == inBundle.TicketData.TicketType);
            if (find == null)
                return null;

            return find.Name;
        }
    }
}
