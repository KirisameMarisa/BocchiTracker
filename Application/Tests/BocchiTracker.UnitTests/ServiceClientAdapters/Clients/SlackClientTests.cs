using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.IO;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientData;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class SlackClientTests
    {
        private string? _channel;
        private AuthConfig? _auth_config;
        private IServiceIssueClient? _client;

        public SlackClientTests()
        {
            {
                var factory = new AuthConfigRepositoryFactory(new PasswordService(new MacAddressProvider()));
                factory.Initialize(Path.Combine("Resources", "Configs", "AuthConfigs"));
                _auth_config = factory.Load(ServiceDefinitions.Slack);
            }

            {
                var factory = new ServiceClientAdapterFactory();
                _client = factory.CreateIssueService(ServiceDefinitions.Slack);
            }

            {
                var repository = new ConfigRepository<ProjectConfig>(new FileSystem());
                var filepath = Path.Combine("Resources", "Configs", "ProjectConfigs", "Test.ProjectConfig.yaml");
                repository.SetLoadFilename(filepath);
                var config = repository.Load();
                _channel = config.GetServiceConfig(ServiceDefinitions.Slack).URL;
            }
        }

        [Fact]
        public async Task Test_Authenticate()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);
        }

        [Fact]
        public async Task Test_Post()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var ticket = new TicketData
            {
                Summary = "Test Summary",
                Description = "Test Description",
            };
            var post_result = await _client.Post(ticket);

            Assert.True(post_result.Item1);
            Assert.NotNull(post_result.Item2);
        }

        [Fact]
        public async Task Test_GetTicketTypes()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var ticket_types = await _client.GetTicketTypes();
            Assert.Null(ticket_types);
        }

        [Fact]
        public async Task Test_GetLabels()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var labels = await _client.GetLabels();
            Assert.Null(labels);
        }

        [Fact]
        public async Task Test_GetPriorities()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var priorities = await _client.GetPriorities();
            Assert.Null(priorities);
        }

        [Fact]
        public async Task Test_GetUsers()
        {
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
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

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var ticket = new TicketData
            {
                Summary = "ファイルアップロードテスト",
                Description = "TestPic.png, log.txtがスレッドにアップロードされます",
            };
            var post_result = await _client.Post(ticket);

            Assert.True(post_result.Item1);
            Assert.NotNull(post_result.Item2);

            bool upload_result = await _client.UploadFiles(post_result.Item2, filenames);
            Assert.True(upload_result);
        }
    }
}
