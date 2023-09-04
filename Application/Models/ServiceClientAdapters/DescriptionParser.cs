using BocchiTracker.ServiceClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IDescriptionParser
    {
        CustomFields Parse(string inDescription);
    }

    public class DescriptionParser : IDescriptionParser
    {
        public CustomFields Parse(string inDescription)
        {
            var result = new CustomFields();

            if (string.IsNullOrEmpty(inDescription))
                return result;

            Dictionary<string, List<string>> parsedData = new Dictionary<string, List<string>>();

            string pattern = @"\<.+?\>";
            MatchCollection matches = Regex.Matches(inDescription, pattern);

            foreach (Match match in matches)
            {
                string content = match.Value;
                content = content.Substring(1, content.Length - 2);

                if (content.Contains(":"))
                {
                    string[] parts = content.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        parsedData[key] = value.Split(",").Select(x => x.Trim()).ToList();
                    }
                }
            }
            if (parsedData.Count() == 0)
                return result;

            result.Set(parsedData);
            return result;
        }
    }
}
