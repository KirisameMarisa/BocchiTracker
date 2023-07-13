using BocchiTracker.Config.Configs;
using BocchiTracker.IssueAssetCollector.Handlers.Coredump;
using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IFilenameGenerator _filenameGenerator;
        private readonly ProjectConfig _projectConfig;

        public CreateActionHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGen, ProjectConfig inConfig)
        {
            _eventAggregator = inEventAggregator;
            _filenameGenerator = inFilenameGen;
            _projectConfig = inConfig;
        }

        public IHandle Create(Type inType)
        {
            if (_cacheHandles.ContainsKey(inType))
                return _cacheHandles[inType];
#if WINDOWS
            if (inType == typeof(LocalScreenshotHandler))
            {
                var handler = new LocalScreenshotHandler(new Utils.Win32.WindowsClientCapture(), new Utils.Win32.GetWindowHandleFromPid(), _filenameGenerator);
                _cacheHandles.Add(inType, handler);
            }
            else
#endif
            if(inType == typeof(RemoteScreenshotHandler))
            {
                var handler = new RemoteScreenshotHandler(_eventAggregator, _filenameGenerator);
                _cacheHandles.Add(inType, handler);
            }
            else if (inType == typeof(WindowsCoredumpHandler))
            {
                var handler = new WindowsCoredumpHandler(_filenameGenerator, _projectConfig.ExternalToolsPath?.ProcDumpPath);
                _cacheHandles.Add(inType, handler);
            }
            return _cacheHandles[inType];
        }
    }
}
