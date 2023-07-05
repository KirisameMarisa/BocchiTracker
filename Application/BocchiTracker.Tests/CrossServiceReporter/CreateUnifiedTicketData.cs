using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.CrossServiceReporter
{

    public class CreateUnifiedTicketData
    {
        public static string ReplaceVariables(string input, Dictionary<string, string> variables)
        {
            var replacedString = input;
            foreach (var variable in variables)
            {
                var placeholder = $"{{{variable.Key}}}";
                replacedString = replacedString.Replace(placeholder, variable.Value);
            }
            return replacedString;
        }

        [Fact]
        public void Test_Handle()
        {
            var input = "hoge test {Description} hoge {PosX}, {PosY}";
            var variables = new Dictionary<string, string>
            {
                { "Description", "AHO" },
                { "PosX", "5" },
                { "PosY", "10" }
            };

            var replacedString = ReplaceVariables(input, variables);
            Console.WriteLine(replacedString);
        }
    }
}
