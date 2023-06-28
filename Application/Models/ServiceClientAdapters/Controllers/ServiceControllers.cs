using BocchiTracker.ProjectConfig;
using BocchiTracker.ServiceClientAdapters.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Controllers
{
    public class TicketPostingController : ControllerBase
    {
        public TicketPostingController(IServiceClientAdapterFactory inFactory)
            : base(inFactory) {}

        public void Post(ServiceDefinitions serviceType, TicketData ticketData)
        {
            var service = _factory.CreateServiceClientAdapter(serviceType);
            service.Post(ticketData);
        }
    }

    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(IServiceClientAdapterFactory inFactory)
            : base(inFactory) { }

        public async Task<bool> Authenticate(ServiceDefinitions inServiceType, AuthConfig inAuthConfig, string inURL, string? inProxy)
        {
            var service = _factory.CreateServiceClientAdapter(inServiceType);
            return await service.Authenticate(inAuthConfig, inURL, inProxy);
        }
    }
}
