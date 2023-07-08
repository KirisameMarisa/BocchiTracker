using System;
using System.IO;

namespace BocchiTracker.IssueAssetCollector
{
    public class IssueAssetMonitor : IDisposable
    {
        public Action<string>?          AddedAction      { get; set; }
        public Action<string>?          DeletedAction    { get; set; }
        public Action<string, string>?  RenamedAction    { get; set; }

        private FileSystemWatcher _watcher;

        public IssueAssetMonitor(string inDirectory, string inExtension)
        {
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
            AddedAction?.Invoke(e.FullPath);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            DeletedAction?.Invoke(e.FullPath);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            RenamedAction?.Invoke(e.OldFullPath, e.FullPath);
        }

        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }
    }
}
