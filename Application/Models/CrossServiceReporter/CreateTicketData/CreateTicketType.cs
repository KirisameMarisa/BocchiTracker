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
            if (inBundle.TicketData.TicketType == null)
                return null;

            var find = inConfig.TicketTypeMappings.Find(x => x.Definition == inBundle.TicketData.TicketType);
            if (find == null)
                return null;

            var ticketType = inBundle.TicketTypeListService.GetData(inService);
            if (ticketType == null)
                return null;

            foreach (var value in ticketType)
            {
                if (value.Name == find.Name)
                    return value.Id;
            }
            return null;
        }
    }
}
