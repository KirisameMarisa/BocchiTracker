using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector
{
    public interface IFilenameGenerator
    {
        string Generate();
    }

    public class TimestampedFilenameGenerator : IFilenameGenerator
    {
        public string Generate()
        {
            return DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
        }
    }
}
