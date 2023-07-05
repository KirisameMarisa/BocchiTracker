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
    public interface ICreateUnifiedTicketData<T>
    {
        T? Create(IssueServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig);
    }
}
