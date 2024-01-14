using BocchiTracker.ApplicationInfoCollector.Handlers;
using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientData;
using System.Runtime.CompilerServices;

namespace BocchiTracker.IssueAssetCollector
{
    public interface IFilenameGenerator
    {
        string Generate(AppStatusBundle inAppStatusBundle);
    }

    public class TimestampedFilenameGenerator : IFilenameGenerator
    {
        public string Generate(AppStatusBundle inAppStatusBundle)
        {
            return DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
        }
    }

    public class RunningAppFilenameGenerator : IFilenameGenerator 
    {
        public string Generate(AppStatusBundle inAppStatusBundle)
        {
            var info = inAppStatusBundle;
            if (info == null)
                return string.Empty;

            return $"{info.AppBasicInfo.AppName}_{info.AppBasicInfo.ClientID}";
        }
    }

    public interface IFilenameGeneratorFactory
    {
        IFilenameGenerator GetFilenameGenerator(Type inType);
    }

    public class FilenameGeneratorFactory : IFilenameGeneratorFactory
    {
        private static Dictionary<Type, IFilenameGenerator> _services = new Dictionary<Type, IFilenameGenerator>(); 
        
        public FilenameGeneratorFactory(AppStatusBundles inAppStatusBundles)
        {
            if (!_services.ContainsKey(typeof(TimestampedFilenameGenerator)))
                _services.Add(typeof(TimestampedFilenameGenerator), new TimestampedFilenameGenerator());

            if (!_services.ContainsKey(typeof(RunningAppFilenameGenerator)))
                _services.Add(typeof(RunningAppFilenameGenerator), new RunningAppFilenameGenerator());
        }

        public IFilenameGenerator GetFilenameGenerator(Type inType)
        {
            return _services[inType];
        }
    }
}
