using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ModelEventBus
{
    public class AppDisconnectEvent : IRequest 
    {
        public int ClientID { get; set; }
    }
}
