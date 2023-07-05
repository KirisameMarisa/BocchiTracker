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
        private string _file_path;
        private IFileSystem _file_system;

        public ConfigRepository(string filePath, IFileSystem inFileSystem)
        {
            _file_path = filePath;
            _file_system = inFileSystem;
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

                using var reader = _file_system.File.OpenText(_file_path);
                var settings = deserializer.Deserialize<T>(reader);

                return settings;
            }
            catch (Exception ex) 
            {
                throw new InvalidDataException($"Failed to deserialize cache file {_file_path}.", ex);
            }
        }

        public void Save(T settings)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            using var writer = _file_system.File.CreateText(_file_path);
            serializer.Serialize(writer, settings);
        }
    }
}
