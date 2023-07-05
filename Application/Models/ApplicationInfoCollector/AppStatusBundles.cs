using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector
{
    public class AppBasicInfo
    {
        public int ClientID { get; set; }

        public string Pid { get; set; } = string.Empty;

        public string AppName { get; set; } = string.Empty;

        public string AppVersion { get; set; } = string.Empty;

        public string Args { get; set; } = string.Empty;

        public string Platform { get; set; } = string.Empty;
    }

    public class AppStatusBundle
    {
        public AppBasicInfo AppBasicInfo { get; set; } = new AppBasicInfo();

        public Dictionary<string, dynamic> AppStatusDynamics { get; set; } = new Dictionary<string, dynamic>();

        public AppStatusBundle(int inClientID) { AppBasicInfo.ClientID = inClientID; }
    }

    public class AppStatusBundles 
    {
        public Dictionary<int, AppStatusBundle> Bundles { get; set; } = new Dictionary<int, AppStatusBundle>();

        public IEnumerable<AppStatusBundle> GetBundlesByAppName(string appName)
        {
            return Bundles.Values.Where(b => b.AppBasicInfo.AppName == appName);
        }

        public AppStatusBundle? GetBundlesByClientID(int inClientID)
        {
            if (Bundles.ContainsKey(inClientID))
                return Bundles[inClientID];
            return null;
        }

        public void Add(int inClientID)
        {
            if (!Bundles.ContainsKey(inClientID))
                Bundles.Add(inClientID, new AppStatusBundle(inClientID));
        }

        public void Remove(int inClientID)
        {
            if (Bundles.ContainsKey(inClientID))
                Bundles.Remove(inClientID);
        }

        public AppStatusBundle this[int inClientID]
        {
            set => this.Bundles[inClientID] = value;
            get => this.Bundles[inClientID];
        }
    }
}
