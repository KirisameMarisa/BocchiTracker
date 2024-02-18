
using System;
using System.Runtime.InteropServices;

namespace BocchiTracker.GameCaptureRTC
{
    public class CaptureFrameStorage
    {
        private int _frameRate = 30;
        private int _recordingMaxFrames = 0;
        public ModelEvent.CaptureStreamParameter CaptureStreamParameter { get; set; } = new ModelEvent.CaptureStreamParameter();

        public CaptureFrameStorage(int inFrameRate, int inRecordingMintes)
        {
            _frameRate = inFrameRate;
            _recordingMaxFrames = (inRecordingMintes * 60) * _frameRate;
        }

        public void AddFrame(int inWidth, int inHeight, int inStride, nint inData)
        {
            if (CaptureStreamParameter.Frames.Count > _recordingMaxFrames)
                CaptureStreamParameter.Frames.RemoveAt(0);

            // メモリ領域のコピーを作成して渡す
            byte[] dataCopy = new byte[inHeight * inStride];
            unsafe
            {
                byte* src = (byte*)inData;
                for (int i = 0; i < dataCopy.Length; i++)
                {
                    dataCopy[i] = *(src + i);
                }
            }

            CaptureStreamParameter.Frames.Add(new ModelEvent.CaptureStreamParameter.Frame
            {
                Data = dataCopy,
                Width = inWidth,
                Height = inHeight,
                Stride = inStride,
            });
        }
    }
}
