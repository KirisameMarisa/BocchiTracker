using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace BocchiTracker.ProcessLinkQuery
{
    public class VariableDump
    {
        public Dictionary<string, List<string>> 
            ClassAndPropertyNames { get; private set; } = new Dictionary<string, List<string>>();

        public VariableDump(string inSchemaJson)
        {
            Analytics(inSchemaJson);
        }

        private void Analytics(string inSchemaJson)
        {
            if (string.IsNullOrEmpty(inSchemaJson))
                return;
            if (!System.IO.File.Exists(inSchemaJson))
                return;

            string content = System.IO.File.ReadAllText(inSchemaJson);
            using JsonDocument document = JsonDocument.Parse(content);
            JsonElement root = document.RootElement;

            if (root.TryGetProperty("definitions", out JsonElement definitions))
            {
                foreach (JsonProperty definition in definitions.EnumerateObject())
                {
                    string className = definition.Name;
                    int index = className.LastIndexOf("_") + 1;
                    className = className.Substring(index, className.Length - index);
                    var propertyNames = new List<string>();

                    if (definition.Value.TryGetProperty("properties", out JsonElement properties))
                    {
                        foreach (JsonProperty property in properties.EnumerateObject())
                        {
                            propertyNames.Add(property.Name);
                        }
                    }

                    if (propertyNames.Count == 0)
                        continue;

                    ClassAndPropertyNames.Add(className, propertyNames);
                }
            }
        }
    }
}
