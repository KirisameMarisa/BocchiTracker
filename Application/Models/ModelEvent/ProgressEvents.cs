using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class ProgressEventParameter
    {
        public string Message { get; set; } = string.Empty;
    }

    public class StartProgressEvent : PubSubEvent<ProgressEventParameter> {}

    public class ProgressingEvent : PubSubEvent<ProgressEventParameter> { }

    public class EndProgressEvent : PubSubEvent {}
}
