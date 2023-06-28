using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.Config;
using BocchiTracker.Config.Configs;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Controllers;
using BocchiTracker.ServiceClientAdapters.Data;


namespace BocchiTracker.IssueInfoCollector.MetaData
{
    public interface IMetaService<T>
    {
        Task Load();

        Task<T?> GetUnifiedData();

        Task<T?> GetData(ServiceDefinitions inServiceType);
    }

    public class UserListService : IMetaService<List<UserData>>
    {
        public Task<List<UserData>?> GetData(ServiceDefinitions inServiceType)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserData>?> GetUnifiedData()
        {
            throw new NotImplementedException();
        }

        public async Task Load()
        {
            var cache = new CacheProvider(new FileSystem());
            var factory = new ServiceClientAdapterFactory();
            var controller = new AuthenticationController(factory);

            bool result = await controller.Authenticate(ServiceDefinitions.Github, new AuthConfig(), "", "");

            DataRepository data_repository = new DataRepository(factory, cache);

            var users = await data_repository.GetUsers(ServiceDefinitions.Github);
        }
    }
}
