using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BocchiTracker.ServiceClientAdapters.Data;
using BocchiTracker.Config.Configs;

namespace BocchiTracker.ServiceClientAdapters.Clients
{
    public interface IServiceIssueClient : IService, IServiceUploadClient
    {
        Task<(bool, string?)>           Post(TicketData inTicketData);

        Task<List<IdentifierData>?>     GetTicketTypes();

        Task<List<IdentifierData>?>     GetLabels();

        Task<List<IdentifierData>?>     GetPriorities();
         
        Task<List<UserData>?>           GetUsers();
    }
}
