using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientData;
using BocchiTracker.ServiceClientData.Configs;
using BocchiTracker.ServiceClientAdapters;
using BocchiTracker.ServiceClientAdapters.Data;


namespace BocchiTracker.IssueInfoCollector.MetaData
{
    public interface IMetaService<T>
    {
        Task            Load(IDataRepository inRepository);

        T?              GetData(ServiceDefinitions inServiceType);

        T?              GetUnifiedData();
    }
}
