using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.Config.Configs;
using BocchiTracker.IssueAssetCollector.Handlers.Coredump;
using BocchiTracker.IssueAssetCollector.Handlers.Log;
using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers
{
    public interface ICreateActionHandler
    {
        IHandle Create(Type inType);
    }

    public class CreateActionHandler : ICreateActionHandler
    {
        private Dictionary<Type, IHandle> _cacheHandles = new Dictionary<Type, IHandle>();
        private readonly IEventAggregator _eventAggregator;
        private readonly IFilenameGeneratorFactory _filenameGeneratorFactory;
        private readonly ProjectConfig _projectConfig;
        private readonly AppStatusBundles _appStatusBundles;

        public CreateActionHandler(IEventAggregator inEventAggregator, IFilenameGeneratorFactory inFilenameGenFac, AppStatusBundles inAppStatusBundles, ProjectConfig inConfig)
        {
            _eventAggregator = inEventAggregator;
            _filenameGeneratorFactory = inFilenameGenFac;
            _projectConfig = inConfig;
            _appStatusBundles = inAppStatusBundles;
        }

        public IHandle Create(Type inType)
        {
            if (_cacheHandles.ContainsKey(inType))
                return _cacheHandles[inType];
#if WINDOWS
            if (inType == typeof(LocalScreenshotHandler))
            {
                var handler = new LocalScreenshotHandler(new Utils.Win32.WindowsClientCapture(), new Utils.Win32.GetWindowHandleFromPid(), _filenameGeneratorFactory.GetFilenameGenerator(typeof(TimestampedFilenameGenerator)));
                _cacheHandles.Add(inType, handler);
            }
            else
#endif
            if(inType == typeof(RemoteScreenshotHandler))
            {
                var handler = new RemoteScreenshotHandler(_eventAggregator, _filenameGeneratorFactory.GetFilenameGenerator(typeof(TimestampedFilenameGenerator)));
                _cacheHandles.Add(inType, handler);
            }
#if WINDOWS
            else if (inType == typeof(WindowsCoredumpHandler))
            {
                var handler = new WindowsCoredumpHandler(_filenameGeneratorFactory.GetFilenameGenerator(typeof(TimestampedFilenameGenerator)), _projectConfig.ExternalToolsPath?.ProcDumpPath);
                _cacheHandles.Add(inType, handler);
            }
#endif
            if(inType == typeof(LogRemoteCaptureHandler))
            {
                var handler = new LogRemoteCaptureHandler(_eventAggregator, _filenameGeneratorFactory.GetFilenameGenerator(typeof(RunningAppFilenameGenerator)));
                _cacheHandles.Add(inType, handler);
            } 
            else if (inType == typeof(LogFileCaptureHandler))
            {
                var handler = new LogFileCaptureHandler(_eventAggregator, _filenameGeneratorFactory.GetFilenameGenerator(typeof(RunningAppFilenameGenerator)));
                _cacheHandles.Add(inType, handler);
            }

            return _cacheHandles[inType];
        }
    }
}
