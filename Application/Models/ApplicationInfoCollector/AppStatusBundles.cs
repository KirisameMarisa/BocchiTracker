using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BocchiTracker.ApplicationInfoCollector
{
    public class AppBasicInfo
    {
        public int ClientID { get; set; }

        public string Pid { get; set; } = string.Empty;

        public string AppName { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string Args { get; set; } = string.Empty;

        public string Platform { get; set; } = string.Empty;

        public void Set(string inName, string inValue)
        {
            if(inName == GetVarName(nameof(ClientID)) && int.TryParse(inValue, out int outClientID))
                ClientID = outClientID;
            else if(inName == GetVarName(nameof(Pid)))
                Pid = inValue;
            else if (inName == GetVarName(nameof(AppName)))
                AppName = inValue;
            else if (inName == GetVarName(nameof(Version)))
                Version = inValue;
            else if (inName == GetVarName(nameof(Args)))
                Args = inValue;
            else if (inName == GetVarName(nameof(Platform)))
                Platform = inValue;
        }

        public Dictionary<string, string> ToDict()
        {
            return new Dictionary<string, string> { 
                { GetVarName(nameof(Pid)),      Pid         },
                { GetVarName(nameof(AppName)),  AppName     },
                { GetVarName(nameof(Version)),  Version     },
                { GetVarName(nameof(Args)),     Args        },
                { GetVarName(nameof(Platform)), Platform    },
            };
        }

        private string GetVarName(string inName)
        {
            const string cPrefix = nameof(AppBasicInfo);
            var snakeCase = Regex.Replace(inName, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
            return $"{cPrefix}.{snakeCase}";
        }
    }

    public class AppStatusBundle
    {
        public AppBasicInfo AppBasicInfo { get; set; } = new AppBasicInfo();

        public Dictionary<string, dynamic> AppStatusDynamics { get; set; } = new Dictionary<string, dynamic>();

        public AppStatusBundle(int inClientID) { AppBasicInfo.ClientID = inClientID; }

        public override string ToString() { return $"{AppBasicInfo.AppName} Ver:{AppBasicInfo.Version} Platform:{AppBasicInfo.Platform}"; }
    }

    public class AppStatusBundles 
    {
        public Action<AppStatusBundle>? AppConnected { get; set; }

        public Action<AppStatusBundle>? AppDisconnected { get; set; }

        public Dictionary<int, AppStatusBundle> Bundles { get; set; } = new Dictionary<int, AppStatusBundle>();

        public AppStatusBundle? TrackerApplication;

        public AppStatusBundle? GetBundlesByClientID(int inClientID)
        {
            if (Bundles.ContainsKey(inClientID))
                return Bundles[inClientID];
            return null;
        }

        public void Add(int inClientID)
        {
            if (!Bundles.ContainsKey(inClientID))
            {
                var item = new AppStatusBundle(inClientID);
                Bundles.Add(inClientID, item);
                AppConnected?.Invoke(item);
            }
        }

        public void Remove(int inClientID)
        {
            if (Bundles.ContainsKey(inClientID))
            {
                var item = Bundles[inClientID];
                AppDisconnected?.Invoke(item);
                Bundles.Remove(inClientID);
            }
        }

        public AppStatusBundle this[int inClientID]
        {
            set => this.Bundles[inClientID] = value;
            get => this.Bundles[inClientID];
        }
    }
}
