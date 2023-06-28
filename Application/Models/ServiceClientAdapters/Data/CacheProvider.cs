using System;
using System.IO;
using System.IO.Abstractions;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Diagnostics;

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
        private string _file_path;
        private IFileSystem _file_system;
        private readonly int _expiry_day;

        public CacheProvider(string inBaseDirectory, IFileSystem inFileSystem, int inExpiryDay = 30)
        {
            _file_path      = Path.Combine(inBaseDirectory, "BocchiTracker", "{0}.Cache.yaml");
            _file_system    = inFileSystem;
            _expiry_day     = inExpiryDay;
        }

        public bool IsExpired(string inLabel)
        {
            string filename = string.Format(_file_path, inLabel);
            if (!_file_system.File.Exists(filename))
            {
                return true;
            }

            DateTime lastModified = _file_system.File.GetLastWriteTime(filename);
            return (DateTime.Now - lastModified).TotalDays > _expiry_day;
        }

        public void Set<T>(string inLabel, T value)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            string filename = string.Format(_file_path, inLabel);
            using var writer = _file_system.File.CreateText(filename);
            serializer.Serialize(writer, value);
        }

        public T Get<T>(string inLabel)
        {
            string filename = string.Format(_file_path, inLabel);
            if (!_file_system.File.Exists(filename))
            {
                throw new FileNotFoundException($"Cache file {filename} not found.");
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using var reader = _file_system.File.OpenText(filename);

            try
            {
                var settings = deserializer.Deserialize<T>(reader);
                return settings;
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                throw new InvalidDataException($"Failed to deserialize cache file {filename}.", ex);
            }
        }

        public bool TryGet<T>(string inLabel, out T? result)
        {
            try
            {
                result = Get<T>(inLabel);
                return true;
            }
            catch (FileNotFoundException)
            {
                result = default(T);
                return false;
            }
            catch (InvalidDataException)
            {
                result = default(T);
                return false;
            }
        }
    }
}
