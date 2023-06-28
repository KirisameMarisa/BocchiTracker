using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Controllers
{
    public class ControllerBase
    {
        protected readonly IServiceClientAdapterFactory _factory;

        public ControllerBase(IServiceClientAdapterFactory factory)
        {
            _factory = factory;
        }
    }
}
