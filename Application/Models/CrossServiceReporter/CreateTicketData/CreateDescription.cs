using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateDescription : ICreateUnifiedTicketData<string>
    {
        public string? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (string.IsNullOrEmpty(inConfig.DescriptionFormat))
                return inBundle.TicketData.Description;

            string description = inConfig.DescriptionFormat;

            Dictionary<string, string> variables = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(inBundle.TicketData.TicketType))
                variables.Add(nameof(inBundle.TicketData.TicketType), inBundle.TicketData.TicketType);

            if (!string.IsNullOrEmpty(inBundle.TicketData.Summary))
                variables.Add(nameof(inBundle.TicketData.Summary), inBundle.TicketData.Summary);

            if (string.IsNullOrEmpty(inBundle.TicketData.Description))
            {
                variables.Add(nameof(inBundle.TicketData.Description), "");
            }
            else
            {
                variables.Add(nameof(inBundle.TicketData.Description), inBundle.TicketData.Description);
            }

            if (inBundle.TicketData.Assign != null && !string.IsNullOrEmpty(inBundle.TicketData.Assign.Name))
                variables.Add(nameof(inBundle.TicketData.Assign), string.Format($"{nameof(inBundle.TicketData.Assign)}: {inBundle.TicketData.Assign.Name}"));

            if (inBundle.TicketData.Labels != null && inBundle.TicketData.Labels.Count != 0)
                variables.Add(nameof(inBundle.TicketData.Labels), string.Format($"{nameof(inBundle.TicketData.Labels)}: {string.Join(", ", inBundle.TicketData.Labels)}"));

            if (!string.IsNullOrEmpty(inBundle.TicketData.Priority))
                variables.Add(nameof(inBundle.TicketData.Priority), string.Format($"{nameof(inBundle.TicketData.Priority)}: {inBundle.TicketData.Priority}"));

            if (inBundle.TicketData.CustomFields != null && inBundle.TicketData.CustomFields.Count != 0)
            {
                foreach (var (key, value) in inBundle.TicketData.CustomFields)
                {
                    string value_str = string.Format($"{key}: {string.Join(", ", value)}");
                    variables.Add(key, value_str);
                }
            }

            foreach (var variable in variables)
            {
                var placeholder = $"{{{variable.Key}}}";
                description = description.Replace(placeholder, variable.Value);
            }
            return description;
        }
    }
}
