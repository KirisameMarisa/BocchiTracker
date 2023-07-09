using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector
{
    public class AssetData
    {
        public string Name { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public AssetData(string inFilename)
        {
            FullName = inFilename;
            Name = Path.GetFileName(FullName);
        }
    }
}
