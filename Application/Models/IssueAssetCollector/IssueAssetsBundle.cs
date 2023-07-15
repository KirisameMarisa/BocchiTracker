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
        public ObservableCollection<AssetData> Bundle = new ObservableCollection<AssetData>();

        public void Add(string inAsset)
        {
            if (!File.Exists(inAsset))
                return;

            var find = Bundle.Where(x => x.FullName == inAsset).FirstOrDefault() ?? null;
            if (find != null)
                return;

            Bundle.Add(new AssetData (inAsset));
        }

        public bool Delete(string inAsset) 
        {
            var find = Bundle.Where(x => x.FullName == inAsset).FirstOrDefault() ?? null;
            if (find == null)
                return false;

            return Bundle.Remove(find); 
        }

        public List<string> GetFiles()
        {
            return Bundle.Select(x => x.FullName).ToList();
        }
    }
}
