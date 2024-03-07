using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Movie
{
    public class GameCaptureFrameConvertMovieProcess
    {

        public string Output { get; set; } = string.Empty;
        private IEventAggregator _eventAggregator;

        public GameCaptureFrameConvertMovieProcess(IEventAggregator inEventAggregator)
        {
            _eventAggregator = inEventAggregator;
            _eventAggregator
                .GetEvent<GameCaptureFinishEvent>()
                .Subscribe(OnGameCaptureFinishEvent, ThreadOption.BackgroundThread);
        }

        public void OnGameCaptureFinishEvent(string inMoviePath)
        {
            File.Copy(inMoviePath, Output, true);
        }
    }

    public class MovieHandler : IHandle
    {
        public IFilenameGenerator _filenameGenerator { private set; get; }
        private IEventAggregator _eventAggregator;
        private GameCaptureFrameConvertMovieProcess _convert_movie_process;

        public MovieHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator)
        {
            _filenameGenerator = inFilenameGenerator;
            _eventAggregator = inEventAggregator;
            _convert_movie_process = new GameCaptureFrameConvertMovieProcess(inEventAggregator);
        }

        public void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {
            this._convert_movie_process.Output = Path.Combine(inOutput, _filenameGenerator.Generate(inAppStatusBundle) + ".mp4");
            _eventAggregator.GetEvent<GameCaptureStopRequest>().Publish();
        }
    }
}
