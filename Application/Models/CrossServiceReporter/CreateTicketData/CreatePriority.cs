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
    public class CreatePriority : ICreateUnifiedTicketData<string>
    {
        public string? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            if (inConfig.PriorityMappings.Count == 0 || inBundle.TicketData.Priority == null)
                return null;

            var find = inConfig.PriorityMappings.Find(x => x.Definition == inBundle.TicketData.Priority);
            if (find == null)
                find = new ValueMapping { Name = inBundle.TicketData.Priority };

            var priorities = inBundle.PriorityListService.GetData(inService);
            if (priorities == null)
                return null;

            foreach(var value in priorities)
            {
                if (value.Name == find.Name)
                    return value.Id;
            }
            return null;
        }
    }
}
