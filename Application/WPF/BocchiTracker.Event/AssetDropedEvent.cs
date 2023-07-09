using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Event
{
    public class AssetDropedEventParameter
    {
        public string[] Files { get; set; }

        public AssetDropedEventParameter(string[] inFiles)
        {
            Files = inFiles;
        }
    }

    public class AssetDropedEvent : PubSubEvent<AssetDropedEventParameter> {}
}
