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
        Dictionary<string, List<string>> Convert(ServiceConfig inServiceConfig, Dictionary<string, List<string>> inCustomFieldLists);
    }

    public class CustomFieldsToAppInfoConverter : ICustomFieldsToAppInfoConverter
    {
        public Dictionary<string, List<string>> Convert(ServiceConfig inServiceConfig, Dictionary<string, List<string>> inCustomFieldLists)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            var queryFieldMappings = inServiceConfig.QueryFieldMappings;
            foreach (var m in queryFieldMappings)
            {
                if (string.IsNullOrEmpty(m.Definition) || string.IsNullOrEmpty(m.Name))
                    continue;

                if(inCustomFieldLists.TryGetValue(m.Name, out var outValues))
                {
                    result.Add(m.Definition, outValues);
                }
            }
            return result;
        }
    }
}
