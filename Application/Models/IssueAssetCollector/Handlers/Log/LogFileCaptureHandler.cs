using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Log
{
    public class LogCopyEvent 
    {
        private string _source_filename { get; set; } = string.Empty;
        private string _output_filename { get; set; } = string.Empty;

        public LogCopyEvent(IEventAggregator inEventAggregator, string inLog, string inOutput)
        {
            _source_filename = inLog;
            _output_filename = inOutput;

            inEventAggregator
                .GetEvent<IssueSubmitPreEvent>()
                .Subscribe(OnCopy, ThreadOption.BackgroundThread);

            //!< First copy to add to attachment list
            OnCopy();
        }

        private void OnCopy()
        {
            try
            {
                File.Copy(_source_filename, _output_filename, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during log copy: {ex.Message}");
            }
        }
    }

    public class LogFileCaptureHandler : IHandle
    {
        private IEventAggregator _event;
        private RunningAppFilenameGenerator? _filename_generator = null;
        private Dictionary<int, LogCopyEvent> _save_process_map = new Dictionary<int, LogCopyEvent>();

        public LogFileCaptureHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator)
        {
            _event = inEventAggregator;
            _filename_generator = inFilenameGenerator as RunningAppFilenameGenerator;

            _event = inEventAggregator;
            _event
                .GetEvent<AppDisconnectEvent>()
                .Subscribe((AppDisconnectEventParameter inParameter) =>
                {
                    if (_save_process_map.ContainsKey(inParameter.ClientID))
                        _save_process_map.Remove(inParameter.ClientID);
                }, ThreadOption.BackgroundThread);
        }

        public void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {
            if (_filename_generator == null) { return; }

            if(!File.Exists(inAppStatusBundle.AppBasicInfo.LogFilepath)) { return; }

            if (!_save_process_map.ContainsKey(inAppStatusBundle.AppBasicInfo.ClientID))
            {
                _save_process_map.Add(
                    inAppStatusBundle.AppBasicInfo.ClientID,
                    new LogCopyEvent(_event, inAppStatusBundle.AppBasicInfo.LogFilepath, Path.Combine(inOutput, _filename_generator.Generate(inAppStatusBundle) + ".txt"))
                );
            }
        }
    }
}
