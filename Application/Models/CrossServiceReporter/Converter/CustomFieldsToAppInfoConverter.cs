using BocchiTracker.ApplicationInfoCollector;
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
        Dictionary<string, List<string>> Convert(ServiceDefinitions inService, Dictionary<string, List<string>> inCustomFieldLists, CustomFieldListService inCustomFieldListService);
    }

    public class CustomFieldsToAppInfoConverter : ICustomFieldsToAppInfoConverter
    {
        public Dictionary<string, List<string>> Convert(ServiceDefinitions inService, Dictionary<string, List<string>> inCustomFieldLists, CustomFieldListService inCustomFieldListService)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            var metaData = inCustomFieldListService.GetData(inService);
            if(metaData == null)
                return result;

            foreach (var m in metaData)
            {
                if (m == null || string.IsNullOrEmpty(m.Name) || string.IsNullOrEmpty(m.Id))
                    continue;

                if(inCustomFieldLists.ContainsKey(m.Name))
                {
                    result.Add(m.Id, inCustomFieldLists[m.Name]);
                }
            }
            return result;
        }
    }
}
