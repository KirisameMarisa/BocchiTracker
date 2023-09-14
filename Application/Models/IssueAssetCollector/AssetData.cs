using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;

namespace BocchiTracker.IssueAssetCollector
{
    public class AssetData
    {
        public List<string> SupportExts { get; private set; } = new List<string> 
        { 
            ".png",
            ".jpg",
            ".log",
            ".mp4"
        };

        public Dictionary<string, ImageEncoder> SupportImages { get; private set; } = new Dictionary<string, ImageEncoder>
        {
            { ".png", new PngEncoder()  },
            { ".jpg", new JpegEncoder() },
        };

        public string Name { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public byte[]? PictureRawData { get; set; }

        public AssetData(string inFilename)
        {
            FullName = inFilename;
            Name = Path.GetFileName(FullName);
            
            var ext = Path.GetExtension(FullName);
            if(SupportExts.Contains(ext))
            {
                Extension = ext;

                if(SupportImages.ContainsKey(ext))
                {
                    using Image<Bgra32> image = Image.Load<Bgra32>(FullName);
                    using MemoryStream stream = new MemoryStream();
                    image.Save(stream, SupportImages[ext]);
                    PictureRawData = stream.ToArray();
                }
            }
        }
    }
}
