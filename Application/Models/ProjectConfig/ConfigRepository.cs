using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.IO.Abstractions;


namespace BocchiTracker.Config
{
    public class ConfigRepository<T>
    {
        private string _filePath;
        private IFileSystem _fileSystem;

        public ConfigRepository(string filePath, IFileSystem inFileSystem)
        {
            _filePath = filePath;
            _fileSystem = inFileSystem;
        }

        public bool TryLoad(out T? outConfig)
        {
            try
            {
                outConfig = Load();
                return true;
            }
            catch (FileNotFoundException)
            {
                outConfig = default;
                return false;
            }
            catch (InvalidDataException)
            {
                outConfig = default;
                return false;
            }
        }

        public T? Load()
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                using var reader = _fileSystem.File.OpenText(_filePath);
                var settings = deserializer.Deserialize<T>(reader);

                return settings;
            }
            catch (Exception ex) 
            {
                throw new InvalidDataException($"Failed to deserialize cache file {_filePath}.", ex);
            }
        }

        public void Save(T settings)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            using var writer = _fileSystem.File.CreateText(_filePath);
            serializer.Serialize(writer, settings);
        }
    }
}
