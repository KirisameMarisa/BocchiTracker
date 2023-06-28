using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ProjectConfig;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class GithubClientTests
    {
        private string?                 _project_url;
        private AuthConfig?             _auth_config;
        private IServiceClientAdapter   _client;

        public GithubClientTests()
        {
            {
                var factory = new AuthConfigRepositoryFactory(Path.Combine("Configs", "AuthConfigs"));
                _auth_config = factory.Load(ServiceDefinitions.Github);
            }

            {
                var factory = new ServiceClientAdapterFactory();
                _client = factory.CreateServiceClientAdapter(ServiceDefinitions.Github);
            }

            {
                var repository = new ConfigRepository(Path.Combine("Configs", "ProjectConfigs", "Test.ProjectConfig.yaml"), new FileSystem());
                var config = repository.Load();
                _project_url = config.GetServiceURL(ServiceDefinitions.Github);
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
                Lables = new List<string> { "Label1", "Label2", "Label3" }
            };
            result = await _client.Post(ticket);
            Assert.True(result);
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
            Assert.Null(ticket_types);
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
            Assert.Null(priorities);
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
