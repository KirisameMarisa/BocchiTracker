﻿using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientData;
using BocchiTracker.Config.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.IO;
using BocchiTracker.ServiceClientAdapters.Clients;
using BocchiTracker.Config;

namespace BocchiTracker.Tests.ServiceClientAdapters.Clients
{
    public class SlackClientTests
    {
        private string? _channel;
        private AuthConfig? _auth_config;
        private IServiceIssueClient? _client;

        public SlackClientTests()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");
            {
                var factory = new AuthConfigRepositoryFactory(new PasswordService());
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

        [SkippableFact]
        public async Task Test_Authenticate()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);
        }

        [SkippableFact]
        public async Task Test_Post()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

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

        [SkippableFact]
        public async Task Test_GetTicketTypes()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var ticket_types = await _client.GetTicketTypes();
            Assert.Null(ticket_types);
        }

        [SkippableFact]
        public async Task Test_GetLabels()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var labels = await _client.GetLabels();
            Assert.Null(labels);
        }

        [SkippableFact]
        public async Task Test_GetPriorities()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
            Assert.True(result);

            var priorities = await _client.GetPriorities();
            Assert.Null(priorities);
        }

        [SkippableFact]
        public async Task Test_GetUsers()
        {
            Skip.If(Environment.GetEnvironmentVariable("Builder") == "1");

            Assert.NotNull(_auth_config);
            Assert.NotNull(_client);

            bool result = await _client.Authenticate(_auth_config, _channel);
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
