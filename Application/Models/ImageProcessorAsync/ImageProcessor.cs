using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using System.IO;
using System.Reflection;
using SixLabors.ImageSharp.PixelFormats;

namespace BocchiTracker.ImageProcessorAsync
{
    public class ImageProcessor
    {
        public readonly static ImageProcessor Instnace = new ImageProcessor();

        private Dictionary<string, ImageEncoder> _support_encoder = new Dictionary<string, ImageEncoder>
        {
            { ".png", new PngEncoder()  },
            { ".jpg", new JpegEncoder() },
        };

        private List<string> ProcessingFiles = new List<string>();

        private object _mutex = new object();

        public byte[]? Load(string inPath)
        {
            if (!File.Exists(inPath))
                return null;

            if (!Begin(inPath))
                return null;

            var ext = Path.GetExtension(inPath);
            if (!_support_encoder.ContainsKey(ext))
                return null;

            using Image<Bgra32> image = Image.Load<Bgra32>(inPath);
            using MemoryStream stream = new MemoryStream();
            image.Save(stream, _support_encoder[ext]);
            var result = stream.ToArray();

            End(inPath);
            return result;
        }

        public byte[]? LoadEmbedded(Stream inStream)
        {
            var image = Image.Load<Rgba32>(inStream);
            using MemoryStream memStream = new MemoryStream();
            image.SaveAsync(memStream, _support_encoder[".png"]);
            var result = memStream.ToArray();

            return result;
        }

        public void Save(string inOutput, byte[] inImageData, int inWidth, int inHeight)
        {
            if (!Begin(inOutput))
                return;

            using var image = Image.LoadPixelData<Byte4>(inImageData, inWidth, inHeight);

            image.SaveAsPng(inOutput);
            End(inOutput);
        }

        private bool Begin(string inPath)
        {
            while (ProcessingFiles.Contains(inPath))
                Thread.Sleep(1000 * 10);

            lock (_mutex)
            {
                ProcessingFiles.Add(inPath);
            }
            return true;
        }

        private void End(string inPath)
        {
            lock (_mutex)
            {
                ProcessingFiles.Remove(inPath);
            }
        }
    }
}