using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEvent
{
    public class GameCaptureStopRequest : PubSubEvent { }

    public class GameCaptureFinishEvent : PubSubEvent<string> { }
}
