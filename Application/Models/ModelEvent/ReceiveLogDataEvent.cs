using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class ReceiveLogDataEventParameter
    {
        public string Log { get; set; }

        public ReceiveLogDataEventParameter(string inLog)
        {
            Log = inLog;
        }
    }

    public class ReceiveLogDataEvent : PubSubEvent<ReceiveLogDataEventParameter> { }
}
