using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueInfoCollector.MetaData;
using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.Converter
{
    public interface ICustomFieldsToAppInfoConverter
    {
        CustomFields Convert(ServiceConfig inServiceConfig, CustomFields inCustomFieldLists);
    }

    public class CustomFieldsToAppInfoConverter : ICustomFieldsToAppInfoConverter
    {
        public CustomFields Convert(ServiceConfig inServiceConfig, CustomFields inCustomFieldLists)
        {
            var result = new CustomFields();

            var queryFieldMappings = inServiceConfig.QueryFieldMappings;
            foreach (var m in queryFieldMappings)
            {
                string definition = m.Definition;
                string name = string.IsNullOrEmpty(m.Name) ? definition : m.Name;

                if (string.IsNullOrEmpty(definition))
                    continue;

                if(inCustomFieldLists.TryGetValue(name, out List<string> outValues))
                {
                    result.Add(definition, outValues);
                }
            }
            return result;
        }
    }
}
