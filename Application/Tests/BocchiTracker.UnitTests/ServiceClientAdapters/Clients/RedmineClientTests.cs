using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.Config;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class RedmineClientTests
    {
        private string? _project_url;
        private AuthConfig? _auth_config;
        private IServiceIssueClient? _client;

        public RedmineClientTests()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");
            {
                var factory = new AuthConfigRepositoryFactory(new PasswordService());
                factory.Initialize(Path.Combine("Resources", "Configs", "AuthConfigs"));
                _auth_config = factory.Load(ServiceDefinitions.Redmine);
            }

            {
                var factory = new ServiceClientAdapterFactory();
                _client = factory.CreateIssueService(ServiceDefinitions.Redmine);
            }

            {
                var repository = new ConfigRepository<ProjectConfig>(new FileSystem());
                repository.SetLoadFilename(Path.Combine("Resources", "Configs", "ProjectConfigs", "Test.ProjectConfig.yaml"));
                var config = repository.Load();
                _project_url = config.GetServiceConfig(ServiceDefinitions.Redmine).URL;
            }
        }

        [SkippableFact]
        public async Task Test_Authenticate()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);
        }

        [SkippableFact]
        public async Task Test_Post()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var ticket = new TicketData
            {
                Summary = "Test Summary",
                Description = "Test Description",
                TicketType = "1",
                CustomFields = new Dictionary<string, List<string>> { { "2", new List<string> { "100" } } }
            };
            var post_result = await _client.Post(ticket);
            Assert.True(post_result.Item1);
            Assert.NotNull(post_result.Item2);
        }

        [SkippableFact]
        public async Task Test_GetTicketTypes()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var ticket_types = await _client.GetTicketTypes();
            Assert.NotNull(ticket_types);
        }

        [SkippableFact]
        public async Task Test_GetLabels()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var labels = await _client.GetLabels();
            Assert.NotNull(labels);
        }

        [SkippableFact]
        public async Task Test_GetPriorities()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var priorities = await _client.GetPriorities();
            Assert.NotNull(priorities);
        }

        [SkippableFact]
        public async Task Test_GetUsers()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var users = await _client.GetUsers();
            Assert.NotNull(users);
        }

        [SkippableFact]
        public async Task Test_UploadFile()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            List<string> filenames = new List<string>()
            {
                Path.Combine("Resources", "UploadFiles", "TestPic.png"),
                Path.Combine("Resources", "UploadFiles", "log.txt"),
            };

            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var issueKey = await _client.Post(new TicketData { Summary = "Uploading Test", TicketType = "1" });
            Assert.True(issueKey.Item1);
            Assert.True(!string.IsNullOrEmpty(issueKey.Item2));

            result = await _client.UploadFiles(issueKey.Item2, filenames);
            Assert.True(result);
        }
    }
}
