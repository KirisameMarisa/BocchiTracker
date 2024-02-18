using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using Prism.Events;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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
                .Subscribe(Handle, ThreadOption.PublisherThread);
        }

        public void Handle(GameCaptureFinishEventParameter inParam)
        {
            var c = inParam.CaptureStreamParameter;
            if (c == null) return;

            int idx = 0;
            unsafe
            {
                string output = Path.Combine(Output, "temp");
                new DirectoryInfo(output).Create();

                foreach (var frame in c.Frames.ToList()) 
                {
                    if (frame.Data == null) continue;

                    Span<byte> dataSpan = new Span<byte>(frame.Data);
                    using (var image = new Image<Rgb24>(frame.Width, frame.Height))
                    {
                        for (int y = 0; y < frame.Height; y++)
                        {
                            for (int x = 0; x < frame.Width; x++)
                            {
                                var index = y * frame.Stride + x * 3;
                                var color = new Rgb24(dataSpan[index + 2], dataSpan[index + 1], dataSpan[index]);
                                image[x, y] = color;
                            }
                        }
                        image.Save(Path.Combine(output, $"{idx++}.png"));
                    }
                }
            }

            //!< 連番テクスチャーをここで動画化する
            //!< TODO

            //!< ここで連番テクスチャを削除する
            //!< TODO
        }
    }

    public class WebRTCHandler : MovieHandler
    {
        private IEventAggregator _eventAggregator;
        private GameCaptureFrameConvertMovieProcess _convert_movie_process;

        public WebRTCHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator) : base(inFilenameGenerator)
        {
            _eventAggregator = inEventAggregator;
            _convert_movie_process = new GameCaptureFrameConvertMovieProcess(inEventAggregator);
        }

        public override void Handle(AppStatusBundle inAppStatusBundle, int inPID, string inOutput)
        {
            this._convert_movie_process.Output = Path.Combine(inOutput, _filenameGenerator.Generate(inAppStatusBundle) + ".mp4");

            _eventAggregator
                .GetEvent<RequestQueryEvent>()
                .Publish(new ScreenshotRequestEventParameter(inAppStatusBundle.AppBasicInfo.ClientID));
        }
    }
}
