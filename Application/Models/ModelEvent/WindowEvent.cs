using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class WindowLocationChangedEvent : PubSubEvent { }

    public class WindowActiveChangedEvent : PubSubEvent<bool> { }

    public class WindowMouseMoveEvent : PubSubEvent { }
}
