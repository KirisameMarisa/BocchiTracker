using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security;
using Octokit;
using YamlDotNet.Serialization;

namespace BocchiTracker.ServiceClientAdapters
{
    public class AuthConfig
    {
        public string? Password { get; set; } = null;

        public string? Username { get; set; } = null;

        [YamlMember(Alias = "APIKey")]
        public string? APIKey { get; set; } = null;
    }
}
