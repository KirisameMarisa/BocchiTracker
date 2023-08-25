using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters;
using Prism.Mvvm;
using System.Diagnostics;

namespace BocchiTracker.Client.ViewModels.UserConfigParts
{
    public interface IConfig
    {
        public void Save(ref bool outIsNeedRestart);
    }
}
