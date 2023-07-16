using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateWatchUser : ICreateUnifiedTicketData<List<UserData>>
    {
        public List<UserData>? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var users = inBundle.UserListService.GetData(inService);
            if (users == null)
                return null;

            if (inBundle.TicketData.Watchers == null)
                return null;

            var watchers = new List<UserData>();
            foreach(var x in inBundle.TicketData.Watchers)
            {
                var foundUser = users.FirstOrDefault(u => u.Equals(x)) ?? null;
                if (foundUser != null)
                    watchers.Add(foundUser);
            }
            return watchers.Count == 0 ? null : watchers;
        }
    }
}
