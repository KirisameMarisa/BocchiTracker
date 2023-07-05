using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.IssueClients;
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

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class RedmineClientTests
    {
        private string? _project_url;
        private AuthConfig? _auth_config;
        private IServiceIssueClient _client;

        public RedmineClientTests()
        {
            {
                var factory = new AuthConfigRepositoryFactory(Path.Combine("Resources", "Configs", "AuthConfigs"));
                _auth_config = factory.Load(IssueServiceDefinitions.Redmine);
            }

            {
                var factory = new ServiceIssueClientAdapterFactory();
                _client = factory.CreateServiceClientAdapter(IssueServiceDefinitions.Redmine);
            }

            {
                var repository = new ConfigRepository<ProjectConfig>(Path.Combine("Resources", "Configs", "ProjectConfigs", "Test.ProjectConfig.yaml"), new FileSystem());
                var config = repository.Load();
                _project_url = config.GetServiceConfig(IssueServiceDefinitions.Redmine).URL;
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
                TicketType = "2",
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
    }
}
