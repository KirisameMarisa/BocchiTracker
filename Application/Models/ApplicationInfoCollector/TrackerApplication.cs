using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector
{
    public class TrackerApplication
    {
        public AppStatusBundles Bundles { get; set; } = new AppStatusBundles();

        public AppStatusBundle? Tracker { private set; get; }

        public void SetTracker(int inClientID)
        {
            Tracker = Bundles.GetBundlesByClientID(inClientID);
        }
    }
}
