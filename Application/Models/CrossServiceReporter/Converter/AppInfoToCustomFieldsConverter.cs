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

            customFields[nameof(inAppBundle.AppBasicInfo.Platform)].Add(inAppBundle.AppBasicInfo.Platform);
            customFields[nameof(inAppBundle.AppBasicInfo.Args)].Add(inAppBundle.AppBasicInfo.Args);
            customFields[nameof(inAppBundle.AppBasicInfo.AppVersion)].Add(inAppBundle.AppBasicInfo.AppVersion);
            customFields[nameof(inAppBundle.AppBasicInfo.AppName)].Add(inAppBundle.AppBasicInfo.AppName);
            customFields[nameof(inAppBundle.AppBasicInfo.Pid)].Add(inAppBundle.AppBasicInfo.Pid.ToString());
            customFields[nameof(inAppBundle.AppBasicInfo.ClientID)].Add(inAppBundle.AppBasicInfo.ClientID.ToString());

            foreach (var (key, value) in inAppBundle.AppStatusDynamics)
                customFields[key].Add(value);

            return customFields;
        }
    }
}
