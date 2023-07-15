using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class TicketData
    {
        public string? Summary { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public UserData? Assign { get; set; } = new UserData();

        public string? Class { get; set; } = string.Empty;

        public List<UserData>? Watchers { get; set; } = new List<UserData>();

        public string? TicketType { get; set; } = string.Empty;

        public string? Priority { get; set; } = string.Empty;

        public Dictionary<string, List<string>>? CustomFields { get; set; } = new Dictionary<string, List<string>>();

        public List<string>? Lables { get; set; } = new List<string>();
    }
}
