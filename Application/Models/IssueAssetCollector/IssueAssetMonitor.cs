using System;
using System.IO;

namespace BocchiTracker.IssueAssetCollector
{
    public class IssueAssetMonitor
    {
        private IssueAssetsBundle _bundle;
        private FileSystemWatcher _watcher;

        public IssueAssetMonitor(string inDirectory, string inExtension, IssueAssetsBundle inBundle)
        {
            _bundle = inBundle;
            _watcher = new FileSystemWatcher
            {
                Path = inDirectory,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = inExtension,
            };

            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;

            _watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _bundle.Add(e.FullPath);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _bundle.Delete(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _bundle.Rename(e.OldFullPath, e.FullPath);
        }
    }
}
