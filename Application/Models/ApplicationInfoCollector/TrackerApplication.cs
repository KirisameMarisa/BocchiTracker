using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector
{
    public class TrackerApplication
    {
        private readonly AppStatusBundles _bundles;

        public AppStatusBundle? Tracker { private set; get; }

        public TrackerApplication(AppStatusBundles inBundles)
        {
            _bundles = inBundles;
        }

        public void SetTracker(int inClientID)
        {
            Tracker = _bundles.GetBundlesByClientID(inClientID);
        }
    }
}
