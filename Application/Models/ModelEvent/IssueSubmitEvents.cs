using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.ModelEvent
{
    public class IssueSubmittedEventParameter
    {
        public Dictionary<ServiceDefinitions, string> IssueIDMap = new Dictionary<ServiceDefinitions, string>();
    }

    public class IssueSubmittedEvent : PubSubEvent<IssueSubmittedEventParameter> { }

    public class IssueSubmitPreEvent : PubSubEvent { }
}
