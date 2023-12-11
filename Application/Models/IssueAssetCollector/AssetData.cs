using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;
using System.Threading.Tasks;
using System.ComponentModel;
using BocchiTracker.ImageProcessorAsync;
using System.Reflection;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Resources;

namespace BocchiTracker.IssueAssetCollector
{
    public class AssetData
    {
        private List<string> _preview_support_exts = new List<string> { ".png", ".jpg" };

        public string Name { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public byte[]? PictureRawData { get; set; }

        public static byte[]? PreviewLoadingRawData { get; set; }

        public AssetData(string inFilename)
        {
            LoadEmbeddedImage();

            FullName = inFilename;
            Name = Path.GetFileName(FullName);

            var ext = Path.GetExtension(FullName);
            if (_preview_support_exts.Contains(ext))
            {
                Extension = ext;
                Task.Factory.StartNew(() => { PictureRawData = ImageProcessor.Instnace.Load(FullName); });
            }
        }

        public bool IsPreviewPictureSupport()
        {
            return _preview_support_exts.Contains(Extension);
        }

        public bool IsPreviewPictureLoadCompleted()
        {
            if(PictureRawData == null) 
                return false;

            return true;
        }

        public void LoadEmbeddedImage()
        {
            if (PreviewLoadingRawData != null)
                return;

            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream("BocchiTracker.IssueAssetCollector.Res.PreviewLoadingIcon.png");
            if (stream == null)
                return;

            PreviewLoadingRawData = ImageProcessor.Instnace.LoadEmbedded(stream);
        }
    }
}
