using BocchiTracker.Config;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.ServiceClientAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Abstractions;

namespace BocchiTracker.IssueInfoCollector.MetaData
{
    public abstract class MetaListService<T> : IMetaService<List<T>>
    {
        protected Dictionary<ServiceDefinitions, List<T>> _data = new Dictionary<ServiceDefinitions, List<T>>();

        public async Task Load(IDataRepository inRepository)
        {
            foreach (ServiceDefinitions serviceType in Enum.GetValues(typeof(ServiceDefinitions)))
            {
                var value = await GetDataAsync(inRepository, serviceType);
                if (value == null)
                    continue;
                _data[serviceType] = value;
            }
        }

        public List<T>? GetData(ServiceDefinitions inServiceType)
        {
            List<T>? result;
            if (_data.TryGetValue(inServiceType, out result))
                return result;

            return null;
        }

        public List<T> GetUnifiedData()
        {
            List<T> result = new List<T>();
            foreach(var cache_items in _data.Values)
            {
                foreach(var item in cache_items)
                {
                    T? found = result.Find(x => x != null && x.Equals(item));
                    if (found == null)
                        result.Add(item);
                }
            }
            return result;
        }

        protected abstract Task<List<T>?> GetDataAsync(IDataRepository dataRepository, ServiceDefinitions serviceType);
    }

    public class TicketTypeListService : MetaListService<IdentifierData>
    {
        protected override async Task<List<IdentifierData>?> GetDataAsync(IDataRepository dataRepository, ServiceDefinitions serviceType)
        {
            return await dataRepository.GetTicketTypes(serviceType);
        }
    }

    public class PriorityListService : MetaListService<IdentifierData>
    {
        protected override async Task<List<IdentifierData>?> GetDataAsync(IDataRepository dataRepository, ServiceDefinitions serviceType)
        {
            return await dataRepository.GetPriorities(serviceType);
        }
    }

    public class LabelListService : MetaListService<IdentifierData>
    {
        protected override async Task<List<IdentifierData>?> GetDataAsync(IDataRepository dataRepository, ServiceDefinitions serviceType)
        {
            return await dataRepository.GetLabels(serviceType);
        }
    }

    public class UserListService : MetaListService<UserData>
    {
        protected override async Task<List<UserData>?> GetDataAsync(IDataRepository dataRepository, ServiceDefinitions serviceType)
        {
            return await dataRepository.GetUsers(serviceType);
        }
    }
}
