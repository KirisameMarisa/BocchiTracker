using YamlDotNet.Serialization;

namespace BocchiTracker.Config.Configs
{
    public class AuthConfig
    {
        public string? Password { get; set; } = null;

        public string? Username { get; set; } = null;

        [YamlMember(Alias = "APIKey")]
        public string? APIKey { get; set; } = null;
    }
}
