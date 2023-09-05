using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.Converter
{
    public interface IAppInfoToCustomFieldsConverter
    {
        CustomFields Convert(AppStatusBundle inAppBundle);
    }

    public class AppInfoToCustomFieldsConverter : IAppInfoToCustomFieldsConverter
    {
        public CustomFields Convert(AppStatusBundle inAppBundle)
        {
            Dictionary<string, List<string>> customFields = new Dictionary<string, List<string>>();
            
            foreach(var (key, value) in inAppBundle.AppBasicInfo.ToDict())
            {
                customFields.Add(key, new List<string> { value });
            }

            foreach (var (key, value) in inAppBundle.AppStatusDynamics)
            {
                if (!customFields.ContainsKey(key))
                    customFields.Add(key, new List<string>());
                customFields[key].Add(value);
            }

            return new CustomFields(customFields);
        }
    }
}
