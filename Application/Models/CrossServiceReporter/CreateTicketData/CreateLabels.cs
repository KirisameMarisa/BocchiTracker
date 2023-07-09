using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
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
            if(inBundle.TicketData?.Lables != null && inBundle.TicketData.Lables.Count != 0)
                return inBundle.TicketData.Lables;
            return null;
        }
    }
}
