using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateTicketData : ICreateUnifiedTicketData<TicketData>
    {
        public TicketData? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var ticketData = new TicketData
            {
                TicketType      = new CreateTicketType().Create(inService, inBundle, inConfig),
                Summary         = new CreateSummary().Create(inService, inBundle, inConfig),
                Assign        = new CreateAssignUser().Create(inService, inBundle, inConfig),
                Watchers       = new CreateWatchUser().Create(inService, inBundle, inConfig),
                CustomFields    = new CreateCustomfields().Create(inService, inBundle, inConfig),
                Labels          = new CreateLabels().Create(inService, inBundle, inConfig),
                Priority        = new CreatePriority().Create(inService, inBundle, inConfig),
                Description     = new CreateDescription().Create(inService, inBundle, inConfig)
            };
            return ticketData;
        }
    }
}
