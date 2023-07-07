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
    public class CreateCustomfields : ICreateUnifiedTicketData<Dictionary<string, List<string>>>
    {
        public Dictionary<string, List<string>>? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.QueryFieldMappings == null || inBundle.TicketData.CustomFields == null)
                return null;

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var (key, custom_field_name) in inConfig.QueryFieldMappings)
            {
                if (!inBundle.TicketData.CustomFields.ContainsKey(key))
                    continue;

                var values = inBundle.TicketData.CustomFields[key];
                if (values == null)
                    continue;

                result[custom_field_name] = values;
            }
            return result;
        }
    }
}
