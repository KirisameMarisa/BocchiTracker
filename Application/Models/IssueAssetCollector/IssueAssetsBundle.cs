using BocchiTracker.ApplicationInfoCollector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BocchiTracker.IssueAssetCollector
{
    public class IssueAssetsBundle
    {
        public List<string> Bundle { get; set; } = new List<string>();

        public void Add(string inAsset)
        {
            if (!File.Exists(inAsset))
                return;

            if (Bundle.Contains(inAsset))
                return;
            
            Bundle.Add(inAsset);
        }

        public bool Delete(string inAsset) 
        {  
            return Bundle.Remove(inAsset); 
        }

        public void Rename(string inOldAsset, string inNewAsset)
        {
            if(Delete(inOldAsset))
            {
                Add(inNewAsset);
            }
        }
    }
}
