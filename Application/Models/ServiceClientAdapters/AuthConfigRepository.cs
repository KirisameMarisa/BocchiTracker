using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO.Abstractions;

namespace BocchiTracker.ServiceClientAdapters
{
    public class AuthConfigRepository
    {
        private string _file_path;
        private IFileSystem _file_system;

        public AuthConfigRepository(string filePath, IFileSystem inFileSystem)
        {
            _file_path = filePath;
            _file_system = inFileSystem;
        }

        public AuthConfig? Load()
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                using var reader = _file_system.File.OpenText(_file_path);
                var settings = deserializer.Deserialize<AuthConfig>(reader);

                return settings;
            }
            catch
            {
                return null;
            }
        }

        public void Save(AuthConfig settings)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            using var writer = _file_system.File.CreateText(_file_path);
            serializer.Serialize(writer, settings);
        }
    }
}
