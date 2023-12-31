﻿using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientData;
using BocchiTracker.IssueInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateCustomfields : ICreateUnifiedTicketData<CustomFields>
    {
        public CustomFields Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.QueryFieldMappings.Count == 0 || inBundle.TicketData.CustomFields == null || inBundle.TicketData.CustomFields.IsEmpty())
                return new CustomFields();

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var mapping in inConfig.QueryFieldMappings)
            {
                if (string.IsNullOrEmpty(mapping.Name))
                    continue;

                var key = mapping.Definition;
                var customFieldName = mapping.Name;
                if (!inBundle.TicketData.CustomFields.ContainsKey(key))
                    continue;

                var values = inBundle.TicketData.CustomFields[key];
                if (values == null)
                    continue;

                result[customFieldName] = values;
            }
            return new CustomFields(result);
        }
    }
}
