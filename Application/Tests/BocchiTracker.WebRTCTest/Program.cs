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
            var eventAggregator = new EventAggregator();
            var recordingController = new RecordingController(eventAggregator);
            var movieSaveProcess = new GameCaptureFrameConvertMovieProcess(eventAggregator);

            eventAggregator.GetEvent<ConfigReloadEvent>().Publish(new ConfigReloadEventParameter
            (
                new Config.Configs.ProjectConfig { 
                    Port = 8888, 
                    CaptureSetting = new Config.Configs.CaptureSetting 
                    {
                        FFmpegPath = @"C:\Users\maris\AppData\Local\Microsoft\WinGet\Packages\Gyan.FFmpeg.Shared_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-6.1.1-full_build-shared\bin",
                        VideoCodecs = SIPSorceryMedia.Abstractions.VideoCodecsEnum.VP8
                    } 
                },
                new Config.Configs.UserConfig { 
                    UserCaptureSetting = new Config.Configs.UserCaptureSetting 
                    { 
                        RecordingMintes = 1, 
                        GameCaptureType = Config.GameCaptureType.WebRTC 
                    } 
                }
            ));

            Console.WriteLine("サーバー接続中...");
            while (!recordingController.IsConnect())
                Thread.Sleep(10);

            Console.WriteLine("キャプチャーを開始しました。");
            {
                recordingController.Start();
                Thread.Sleep(5000);
                recordingController.Stop();
            }
            Console.WriteLine("キャプチャーを停止しました。");
        }
    }
}