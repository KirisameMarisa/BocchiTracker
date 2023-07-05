using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters.IssueClients;
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
        Task<List<IdentifierData>?>     GetTicketTypes(IssueServiceDefinitions inServiceType);

        Task<List<IdentifierData>?>     GetLabels(IssueServiceDefinitions inServiceType);

        Task<List<IdentifierData>?>     GetPriorities(IssueServiceDefinitions inServiceType);

        Task<List<UserData>?>           GetUsers(IssueServiceDefinitions inServiceType);
    }

    public class DataRepository : IDataRepository
    {
        private readonly IServiceIssueClientFactory _service_client_adapter_factory;
        private readonly ICacheProvider _cache_provider;

        public DataRepository(IServiceIssueClientFactory inServiceClientAdapterFactory, ICacheProvider inCacheProvider)
        {
            _service_client_adapter_factory = inServiceClientAdapterFactory;
            _cache_provider                 = inCacheProvider;
        }

        public async Task<List<IdentifierData>?> GetLabels(IssueServiceDefinitions inServiceType)
        {
            var cache_name = $"{inServiceType}.Labels";
            List<IdentifierData>? result;
            if (_cache_provider.IsExpired(cache_name) || !_cache_provider.TryGet(cache_name, out result))
            {
                var client = _service_client_adapter_factory.CreateServiceClientAdapter(inServiceType);
                if (client == null)
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetLabels();
                _cache_provider.Set(cache_name, result);
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetPriorities(IssueServiceDefinitions inServiceType)
        {
            var cache_name = $"{inServiceType}.Priorities";
            List<IdentifierData>? result;
            if (_cache_provider.IsExpired(cache_name) || !_cache_provider.TryGet(cache_name, out result))
            {
                var client = _service_client_adapter_factory.CreateServiceClientAdapter(inServiceType);
                if (client == null)
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetPriorities();
                _cache_provider.Set(cache_name, result);
            }
            return result;
        }

        public async Task<List<IdentifierData>?> GetTicketTypes(IssueServiceDefinitions inServiceType)
        {
            var cache_name = $"{inServiceType}.TicketTypes";
            List<IdentifierData>? result;
            if (_cache_provider.IsExpired(cache_name) || !_cache_provider.TryGet(cache_name, out result))
            {
                var client = _service_client_adapter_factory.CreateServiceClientAdapter(inServiceType);
                if (client == null)
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetTicketTypes();
                _cache_provider.Set(cache_name, result);
            }
            return result;
        }

        public async Task<List<UserData>?> GetUsers(IssueServiceDefinitions inServiceType)
        {
            var cache_name = $"{inServiceType}.Users";
            List<UserData>? result;
            if(_cache_provider.IsExpired(cache_name) || !_cache_provider.TryGet(cache_name, out result))
            {
                var client = _service_client_adapter_factory.CreateServiceClientAdapter(inServiceType);
                if (client == null)
                {
                    Trace.TraceError($"Cannt get {inServiceType}Client");
                    return null;
                }

                result = await client.GetUsers();
                _cache_provider.Set(cache_name, result);
            }
            return result;
        }
    }
}
