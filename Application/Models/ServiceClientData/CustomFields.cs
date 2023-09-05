using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientData
{
    public class CustomFields
    {
        public Dictionary<string, List<string>> Fields { get; set; } = new Dictionary<string, List<string>>();

        public CustomFields() {}

        public CustomFields(Dictionary<string, List<string>> inFields)
        {
            Fields = inFields;
        }

        public bool TryGetValue(string inKey, out float outValue)
        {
            outValue = default;

            if (!ContainsKey(inKey))
                return false;

            var values = Fields[inKey];
            if(values.Count == 0) 
                return false;

            if (float.TryParse(values[0], out outValue))
                return true;
            return true;
        }

        public bool TryGetValue(string inKey, out int outValue)
        {
            outValue = default;

            if (!ContainsKey(inKey))
                return false;

            var values = Fields[inKey];
            if (values.Count == 0)
                return false;

            if (int.TryParse(values[0], out outValue))
                return true;
            return true;
        }

        public bool TryGetValue(string inKey, out double outValue)
        {
            outValue = default;

            if (!ContainsKey(inKey))
                return false;

            var values = Fields[inKey];
            if (values.Count == 0)
                return false;

            if (double.TryParse(values[0], out outValue))
                return true;
            return true;
        }

        public bool TryGetValue(string inKey, out string outValue)
        {
            outValue = string.Empty;

            if (!ContainsKey(inKey))
                return false;

            var values = Fields[inKey];
            if (values.Count == 0)
                return false;

            outValue = values[0];
            return true;
        }

        public bool TryGetValue(string inKey, out List<string> outValue)
        {
            outValue = new List<string>();

            if (!ContainsKey(inKey))
                return false;

            outValue = Fields[inKey];
            return true;
        }

        public bool ContainsKey(string inKey)
        {
            return Fields.ContainsKey(inKey);
        }

        public void Add(string inKey, string inValue)
        {
            if(string.IsNullOrEmpty(inValue)) 
                return;

            if (!ContainsKey(inKey))
                Fields.Add(inKey, new List<string>());

            Fields[inKey].Add(inValue);
        }

        public void Add(string inKey, List<string> inValues)
        {
            if (!ContainsKey(inKey))
                Fields.Add(inKey, new List<string>());

            foreach(var item in inValues)
                Add(inKey, item);
        }

        public bool IsEmpty()
        {
            return Fields.Count == 0;
        }

        public bool IsNotEmpty()
        {
            return Fields.Count != 0;
        }

        public int Count() => Fields.Count;

        public List<string> this[string inKey]
        {
            set => Add(inKey, value);
            get
            {
                if (TryGetValue(inKey, out List<string> outValue))
                    return outValue;
                return new List<string>();
            }
        }
    }
}
