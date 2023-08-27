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
using System.Collections.ObjectModel;
using BocchiTracker.Config;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class GithubClientTests
    {
        private string?                 _project_url;
        private AuthConfig?             _auth_config;
        private IServiceIssueClient?    _client;

        public GithubClientTests()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");
            {
                var factory = new AuthConfigRepositoryFactory(new PasswordService());
                factory.Initialize(Path.Combine("Resources", "Configs", "AuthConfigs"));
                _auth_config = factory.Load(ServiceDefinitions.Github);
            }

            {
                var factory = new ServiceClientAdapterFactory();
                _client = factory.CreateIssueService(ServiceDefinitions.Github);
            }

            {
                var repository = new ConfigRepository<ProjectConfig>(new FileSystem());
                repository.SetLoadFilename(Path.Combine("Resources", "Configs", "ProjectConfigs", "Test.ProjectConfig.yaml"));
                var config = repository.Load();
                _project_url = config.GetServiceConfig(ServiceDefinitions.Github).URL;
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
                Lables = new List<string> { "Label1", "Label2", "Label3" }
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
            Assert.Null(ticket_types);
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
            Assert.Null(priorities);
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

        [Fact]
        public async Task Test_GetTickets()
        {
            Assert.NotNull(_project_url);
            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _project_url);
            Assert.True(result);

            var issues = _client.GetIssues();
            Assert.NotNull(issues);

            List<TicketData> tickets = new List<TicketData>();
            await foreach (var issue in issues)
            {
                if (issue == null) continue;
                tickets.Add(issue);
            }
            Assert.True(tickets.Count > 0);
        }
    }
}
