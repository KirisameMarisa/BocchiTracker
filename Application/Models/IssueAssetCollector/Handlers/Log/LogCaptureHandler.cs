﻿using BocchiTracker.IssueAssetCollector.Handlers.Screenshot;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Log
{
    public class LogAppendProcess
    {
        private string _output_filename { get; set; } = string.Empty;

        private IEventAggregator _event;

        public LogAppendProcess(IEventAggregator inEventAggregator, string inOutput)
        {
            _output_filename = inOutput;
            _event = inEventAggregator;
            _event
                .GetEvent<ReceiveLogDataEvent>()
                .Subscribe(Handle, ThreadOption.PublisherThread);
        }

        public void Handle(ReceiveLogDataEventParameter inEvent)
        {
            File.AppendAllText(_output_filename, string.Join("\n", inEvent.Log));
        }
    }

    public class LogCaptureHandler : IHandle
    {
        private IEventAggregator _event;
        private RunningAppFilenameGenerator? _filename_generator = null;
        private Dictionary<int, LogAppendProcess> _save_process_map = new Dictionary<int, LogAppendProcess>();

        public LogCaptureHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator)
        {
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

        public void Handle(int inClientID, int inPID, string inOutput)
        {
            if(_filename_generator == null) { return; }

            if(!_save_process_map.ContainsKey(inClientID)) 
            {
                _save_process_map.Add(
                    inClientID,
                    new LogAppendProcess(_event, Path.Combine(inOutput, _filename_generator.Generate() + ".txt"))
                );
            }
        }
    }
}
