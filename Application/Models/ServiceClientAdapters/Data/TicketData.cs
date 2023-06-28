using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class CustomField
    {
        public string? Name { get; set; }

        public List<string>? Values { get; set; }
    }

    public class TicketData
    {
        public string? Project { get; set; }

        public string? Summary { get; set; }

        public string? Description { get; set; }

        public string? Assignee { get; set; }

        public List<string>? Watcheres { get; set; }

        public string? TicketType { get; set; }

        public string? Priority { get; set; }

        public List<CustomField>? CustomFields { get; set; }

        public List<string>? Lables { get; set; }
    }
}
