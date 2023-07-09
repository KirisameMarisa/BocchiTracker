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
    public class CreateWatchUser : ICreateUnifiedTicketData<List<string>>
    {
        public List<string>? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var users = inBundle.UserListService.GetData(inService);
            if (users == null)
                return null;

            if (inBundle.TicketData.Watcheres == null)
                return null;

            var watchers = new List<string>();
            foreach(var x in inBundle.TicketData.Watcheres)
            {
                var foundUser = users.FirstOrDefault(u => u.Name == x)?.Id ?? null;
                if (foundUser != null)
                    watchers.Add(foundUser);
            }
            return watchers.Count == 0 ? null : watchers;
        }
    }
}
