﻿using BocchiTracker.ServiceClientAdapters.Data;
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
                var factory = new AuthConfigRepositoryFactory(Path.Combine("Resources", "Configs", "AuthConfigs"));
                _auth_config = factory.Load(ServiceDefinitions.Slack);
            }

            {
                var factory = new ServiceClientAdapterFactory();
                _client = factory.CreateIssueService(ServiceDefinitions.Slack);
            }

            {
                var repository = new ConfigRepository<ProjectConfig>(Path.Combine("Resources", "Configs", "ProjectConfigs", "Test.ProjectConfig.yaml"), new FileSystem());
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
            Assert.Equal("success", post_result.Item2);
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
    }
}