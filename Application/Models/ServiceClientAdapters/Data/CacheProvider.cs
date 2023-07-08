using System;
using System.IO;
using System.IO.Abstractions;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Diagnostics;
using System.Collections.Generic;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public interface ICacheProvider
    {
        bool    IsExpired(string inLabel);

        bool    TryGet<T>(string inLabel, out T? outResult);

        T       Get<T>(string inLabel);

        void    Set<T>(string inLabel, T value);
    }

    public class CacheProvider : ICacheProvider
    {
        private string _filePath;
        private IFileSystem _fileSystem;
        private readonly int _expiryDay;
        private readonly Dictionary<string, object> _cache;

        public CacheProvider(string inBaseDirectory, IFileSystem inFileSystem, int inExpiryDay = 30)
        {
            _filePath      = Path.Combine(inBaseDirectory, "BocchiTracker", "{0}.Cache.yaml");
            _fileSystem    = inFileSystem;
            _expiryDay     = inExpiryDay;
            _cache          = new Dictionary<string, object>();
        }

        public bool IsExpired(string inLabel)
        {
            string filename = string.Format(_filePath, inLabel);
            if (!_fileSystem.File.Exists(filename))
            {
                return true;
            }

            DateTime lastModified = _fileSystem.File.GetLastWriteTime(filename);
            return (DateTime.Now - lastModified).TotalDays > _expiryDay;
        }

        public void Set<T>(string inLabel, T value)
        {
            if (value == null)
                return;

            string filename = string.Format(_filePath, inLabel);
            var dir = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(dir))
                return;

            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            _fileSystem.Directory.CreateDirectory(dir);
            using var writer = _fileSystem.File.CreateText(filename);
            serializer.Serialize(writer, value);

            _cache[inLabel] = value;
        }

        public T Get<T>(string inLabel)
        {
            if (_cache.TryGetValue(inLabel, out object? cachedValue))
            {
                return (T)cachedValue;
            }

            string filename = string.Format(_filePath, inLabel);
            if (!_fileSystem.File.Exists(filename))
            {
                throw new FileNotFoundException($"Cache file {filename} not found.");
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using var reader = _fileSystem.File.OpenText(filename);

            try
            {
                var settings = deserializer.Deserialize<T>(reader);
                if (settings != null)
                    _cache[inLabel] = settings;
                return settings;
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                throw new InvalidDataException($"Failed to deserialize cache file {filename}.", ex);
            }
        }

        public bool TryGet<T>(string inLabel, out T? outResult)
        {
            try
            {
                outResult = Get<T>(inLabel);
                return true;
            }
            catch (FileNotFoundException)
            {
                outResult = default(T);
                return false;
            }
            catch (InvalidDataException)
            {
                outResult = default(T);
                return false;
            }
        }
    }
}
