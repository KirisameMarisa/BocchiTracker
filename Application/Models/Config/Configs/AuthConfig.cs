using YamlDotNet.Serialization;

namespace BocchiTracker.Config.Configs
{
    public class AuthConfig
    {
        public string? Password { get; set; }

        public string? Username { get; set; }

        [YamlMember(Alias = "APIKey")]
        public string? APIKey { get; set; }
    }
}
