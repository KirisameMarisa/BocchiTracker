using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BocchiTracker.ProjectConfig
{
    public class ConfigRepository
    {
        private string _file_path;
        private IFileSystem _file_system;

        public ConfigRepository(string filePath, IFileSystem inFileSystem)
        {
            _file_path = filePath;
            _file_system = inFileSystem;
        }

        public Config? Load()
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                using var reader = _file_system.File.OpenText(_file_path);
                var settings = deserializer.Deserialize<Config>(reader);

                return settings;
            }
            catch
            {
                return null;
            }
        }

        public void Save(Config settings)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            using var writer = _file_system.File.CreateText(_file_path);
            serializer.Serialize(writer, settings);
        }
    }
}
