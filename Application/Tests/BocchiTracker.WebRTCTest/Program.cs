using BocchiTracker.GameCaptureRTC;
using BocchiTracker.IssueAssetCollector.Handlers.Movie;
using BocchiTracker.ModelEvent;
using Prism.Events;
using System;
using System.Threading;

namespace BocchiTracker.WebRTCTest
{
    class Program
    {
        static void Main()
        {
            string ffmpeg = "put your ffmpeg path";

            var eventAggregator = new EventAggregator();
            var recordingController = new RecordingController(eventAggregator);
            var movieSaveProcess = new GameCaptureFrameConvertMovieProcess(eventAggregator, ffmpeg);

            var p_config = new Config.Configs.ProjectConfig
            {
                CaptureSetting = new Config.Configs.CaptureSetting
                {
                    FFmpegPath = ffmpeg,
                    VideoCodecs = SIPSorceryMedia.Abstractions.VideoCodecsEnum.VP8
                }
            };
            var u_config = new Config.Configs.UserConfig
            {
                UserCaptureSetting = new Config.Configs.UserCaptureSetting
                {
                    RecordingMintes = 1,
                    GameCaptureType = Config.GameCaptureType.WebRTC
                }
            };

            Console.WriteLine("サーバー接続中...");
            while (!recordingController.IsConnect())
                Thread.Sleep(10);

            Console.WriteLine("キャプチャーを開始しました。");
            {
                recordingController.Start(p_config.WebSocketPort, p_config, u_config);
                Thread.Sleep(5000);
                recordingController.Stop();
            }
            Console.WriteLine("キャプチャーを停止しました。");
        }
    }
}