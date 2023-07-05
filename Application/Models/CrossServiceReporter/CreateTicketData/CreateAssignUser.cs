using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateAssignUser : ICreateUnifiedTicketData<string>
    {
        public string? Create(IssueServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var users = inBundle.UserListService.GetData(inService);
            if (users == null)
            {
                Trace.TraceError("user list is null");
                return null;
            }

            if (string.IsNullOrEmpty(inBundle.TicketData.Assignee))
            {
                Trace.TraceError("assignee null");
                return null;
            }

            return users.FirstOrDefault(x => x.Name == inBundle.TicketData.Assignee)?.Id ?? null;
        }
    }
}
