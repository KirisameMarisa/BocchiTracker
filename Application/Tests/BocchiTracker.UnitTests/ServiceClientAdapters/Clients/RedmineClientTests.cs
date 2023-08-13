using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class RedmineClientTests
    {
        private string? _project_url;
        private AuthConfig? _auth_config;
        private IServiceIssueClient? _client;

        public RedmineClientTests()
        {
            {
                var factory = new AuthConfigRepositoryFactory(Path.Combine("Resources", "Configs", "AuthConfigs"));
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

        [Fact]
        public async Task Test_Authenticate()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);
        }

        [Fact]
        public async Task Test_Post()
        {
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

        [Fact]
        public async Task Test_GetTicketTypes()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var ticket_types = await _client.GetTicketTypes();
            Assert.NotNull(ticket_types);
        }

        [Fact]
        public async Task Test_GetLabels()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var labels = await _client.GetLabels();
            Assert.NotNull(labels);
        }

        [Fact]
        public async Task Test_GetPriorities()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var priorities = await _client.GetPriorities();
            Assert.NotNull(priorities);
        }

        [Fact]
        public async Task Test_GetUsers()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var users = await _client.GetUsers();
            Assert.NotNull(users);
        }

        [Fact]
        public async Task Test_UploadFile()
        {
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

            result = await _client.UploadFiles("29", filenames);
            Assert.True(result);
        }
    }
}
