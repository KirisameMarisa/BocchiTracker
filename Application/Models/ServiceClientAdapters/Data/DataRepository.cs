using BocchiTracker.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public interface IDataRepository
    {
        Task<List<IdentifierData>?>     GetTicketTypes(ServiceDefinitions inServiceType);

        Task<List<IdentifierData>?>     GetLabels(ServiceDefinitions inServiceType);

        Task<List<IdentifierData>?>     GetPriorities(ServiceDefinitions inServiceType);

        Task<List<UserData>?>           GetUsers(ServiceDefinitions inServiceType);
    }

    public class DataRepository : IDataRepository
    {
        private readonly IServiceClientFactory _serviceClientFactory;
        private readonly ICacheProvider _cacheProvider;

        public DataRepository(IServiceClientFactory inServiceClientAdapterFactory, ICacheProvider inCacheProvider)
        {
            _serviceClientFactory = inServiceClientAdapterFactory;
            _cacheProvider        = inCacheProvider;
        }

        public async Task<List<IdentifierData>?> GetLabels(ServiceDefinitions inServiceType)
        {
            var cacheName = $"{inServiceType}.Labels";
            List<IdentifierData>? result;
            if (_cacheProvider.IsExpired(cacheName) || !_cacheProvider.TryGet(cacheName, out result))
            {
                var client = _serviceClientFactory.CreateIssueService(inServiceType);
                if (client == null || !client.IsAuthenticated())
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetLabels();
                _cacheProvider.Set(cacheName, result);
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetPriorities(ServiceDefinitions inServiceType)
        {
            var cacheName = $"{inServiceType}.Priorities";
            List<IdentifierData>? result;
            if (_cacheProvider.IsExpired(cacheName) || !_cacheProvider.TryGet(cacheName, out result))
            {
                var client = _serviceClientFactory.CreateIssueService(inServiceType);
                if (client == null || !client.IsAuthenticated())
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetPriorities();
                _cacheProvider.Set(cacheName, result);
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetTicketTypes(ServiceDefinitions inServiceType)
        {
            var cacheName = $"{inServiceType}.TicketTypes";
            List<IdentifierData>? result;
            if (_cacheProvider.IsExpired(cacheName) || !_cacheProvider.TryGet(cacheName, out result))
            {
                var client = _serviceClientFactory.CreateIssueService(inServiceType);
                if (client == null || !client.IsAuthenticated())
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetTicketTypes();
                _cacheProvider.Set(cacheName, result);
            }
            return result;
        }

        public async Task<List<UserData>?> GetUsers(ServiceDefinitions inServiceType)
        {
            var cacheName = $"{inServiceType}.Users";
            List<UserData>? result;
            if(_cacheProvider.IsExpired(cacheName) || !_cacheProvider.TryGet(cacheName, out result))
            {
                var client = _serviceClientFactory.CreateIssueService(inServiceType);
                if (client == null || !client.IsAuthenticated())
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetUsers();
                _cacheProvider.Set(cacheName, result);
            }
            return result;
        }
    }
}
