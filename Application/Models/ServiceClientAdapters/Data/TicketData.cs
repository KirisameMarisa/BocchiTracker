using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class TicketData
    {
        public string? Summary { get; set; }

        public string  Description { get; set; } = string.Empty;

        public string? Assignee { get; set; }

        public List<string>? Watcheres { get; set; }

        public string? TicketType { get; set; }

        public string? Priority { get; set; }

        public Dictionary<string, List<string>>? CustomFields { get; set; }

        public List<string>? Lables { get; set; }
    }
}
