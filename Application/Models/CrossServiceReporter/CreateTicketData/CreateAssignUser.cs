using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using BocchiTracker.ServiceClientAdapters.Data;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateAssignUser : ICreateUnifiedTicketData<UserData>
    {
        public UserData? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var users = inBundle.UserListService.GetData(inService);
            if (users == null)
            {
                Trace.TraceError("user list is null");
                return null;
            }

            if (inBundle.TicketData.Assign == null)
            {
                Trace.TraceError("assignee null");
                return null;
            }

            return users.FirstOrDefault(x => x.Equals(inBundle.TicketData.Assign)) ?? null;
        }
    }
}
