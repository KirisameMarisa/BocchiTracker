using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.Converter
{
    public interface IAppInfoToCustomFieldsConverter
    {
        Dictionary<string, List<string>> Convert(AppStatusBundle inAppBundle);
    }

    public class AppInfoToCustomFieldsConverter : IAppInfoToCustomFieldsConverter
    {
        public Dictionary<string, List<string>> Convert(AppStatusBundle inAppBundle)
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

            return customFields;
        }
    }
}
