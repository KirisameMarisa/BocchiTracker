using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateLabels : ICreateUnifiedTicketData<List<string>>
    {
        public List<string>? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if(inBundle.TicketData?.Labels != null && inBundle.TicketData.Labels.Count != 0)
                return inBundle.TicketData.Labels;
            return null;
        }
    }
}
