using System.Collections.Generic;

namespace BocchiTracker.Config.Configs
{
    public class ProjectConfig
    {
        public List<Dictionary<ServiceDefinitions, string>>? ServiceURLs { get; set; }

        public string? GetServiceURL(ServiceDefinitions inServiceDefinitions)
        {
            if (ServiceURLs == null)
                return null;

            foreach (var item in ServiceURLs)
            {
                if (item.ContainsKey(inServiceDefinitions))
                {
                    return item[inServiceDefinitions];
                }
            }
            return null;
        }
    }
}