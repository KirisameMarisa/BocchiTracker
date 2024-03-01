
using BocchiTracker.ModelEvent;
using FFMpegCore;
using OpenCvSharp;
using Prism.Events;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.GameCaptureRTC
{
    public class CaptureFrameStorage : IDisposable
    {
        private object _mutext = new object();

        private int _maxRecordingFrameCount = 0;
        private int _maxSplitFrameCount = 0;
        private int _curFrameCount = 0;
        private int _curSpliteFrameCount = 0;

        private string _tempCancatMovieDirectory = Path.Combine(Path.GetTempPath(), "BocchiTracker", "temp", "concat_movies");
        private string _tempMovieDirectory = Path.Combine(Path.GetTempPath(), "BocchiTracker", "temp", "movies");
        private string _tempPicsDirectory = Path.Combine(Path.GetTempPath(), "BocchiTracker", "temp", "pics");

        private int _movieID = 0;
        private int _adjustedwidth = 640;
        private int _adjustedHeight = 480;

        private VideoWriter _videoWriter = default!;

        public CaptureFrameStorage(string inFFmpegPath, int inMaxRecordingFrameCount, int inMaxSplitFrameCount)
        {
            GlobalFFOptions.Configure(options => options.BinaryFolder = inFFmpegPath);

            _maxSplitFrameCount = inMaxSplitFrameCount;
            _maxRecordingFrameCount = inMaxRecordingFrameCount;

            if (!Directory.Exists(_tempMovieDirectory))
                Directory.CreateDirectory(_tempMovieDirectory);
            if (!Directory.Exists(_tempCancatMovieDirectory))
                Directory.CreateDirectory(_tempCancatMovieDirectory);
            if (!Directory.Exists(_tempPicsDirectory))
                Directory.CreateDirectory(_tempPicsDirectory);

            Cleanup();
        }

        public void AddFrame(int inWidth, int inHeight, int inStride, nint inData)
        {
            lock(_mutext)
            {
                if(_adjustedwidth > inWidth || _adjustedwidth == 0)
                    _adjustedwidth = inWidth % 2 == 0 ? inWidth : inWidth - 1;
                if (_adjustedHeight > inHeight || _adjustedHeight == 0)
                    _adjustedHeight = inHeight % 2 == 0 ? inHeight : inHeight - 1;

                if (_curSpliteFrameCount == 0)
                {
                    System.Console.WriteLine("start video capture");
                    if (_videoWriter == null || _videoWriter.IsDisposed)
                        _videoWriter = new VideoWriter();
                    _videoWriter.Open(Path.Combine(_tempMovieDirectory, $"movie.{_movieID}.mp4"), FourCC.MPG4, 30, new OpenCvSharp.Size(inWidth, inHeight));
                    ++_movieID;
                }

                unsafe
                {
                    byte[] dataCopy = new byte[inHeight * inStride];
                    unsafe
                    {
                        byte* src = (byte*)inData;
                        for (int i = 0; i < dataCopy.Length; i++)
                        {
                            dataCopy[i] = *(src + i);
                        }
                    }

                    using (var mat = new Mat(inHeight, inWidth, MatType.CV_8UC3, dataCopy, inStride))
                    {
                        _videoWriter.Write(mat);

                        _curFrameCount++;
                        _curSpliteFrameCount++;
                    }

                }

                if (_curSpliteFrameCount > _maxSplitFrameCount)
                {
                    System.Console.WriteLine("_videoWriter wrote maximum frame, so next video...");
                    _curSpliteFrameCount = 0;
                    _videoWriter.Dispose();
                }

                if (_curFrameCount > _maxRecordingFrameCount)
                {
                    var bochi_files = Directory.GetFiles(_tempMovieDirectory, "*.mp4")
                            .OrderBy(file => int.Parse(Regex.Match(file, @"(?<=movie\.)(\d+)(?=\.mp4)").Value))
                            .ToList();
                    if(bochi_files.Any())
                    {
                        System.Console.WriteLine("the maximum recording time has been exceeded, so remove old video file");
                        File.Delete(bochi_files[0]);
                    }
                    _curFrameCount -= _maxSplitFrameCount;
                }
            }

        }

        public string ConcatMovie()
        {
            lock (_mutext)
            {
                Dispose();
                _curFrameCount = 0;
                _curSpliteFrameCount = 0;

                var output = Path.Combine(_tempCancatMovieDirectory, "bocchi_movie.mp4");
                var bochi_files = Directory.GetFiles(_tempMovieDirectory, "*.mp4");
                if (!bochi_files.Any())
                    return string.Empty;

                var command = FFMpegArguments
                    .FromDemuxConcatInput(bochi_files)
                    .OutputToFile(output, overwrite: true, op => op.Resize(_adjustedwidth, _adjustedHeight));
                bool ret = command.ProcessSynchronously();
                if (ret)
                    Cleanup();
                return output;
            }
        }

        public void Cleanup()
        {
            var bochi_files = Directory.GetFiles(_tempMovieDirectory, "*.mp4");
            foreach (var file in bochi_files)
            {
                File.Delete(file);
            }
        }
    
        public void Dispose()
        {
            if (_videoWriter != null && !_videoWriter.IsDisposed)
            {
                _videoWriter.Dispose();
            }
        }
    }
}
