using System.Collections.Generic;

namespace BocchiTracker.ServiceClientData
{
    public class TicketData
    {
        public string Id { get; set; } = string.Empty;

        public string? Summary { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public UserData? Assign { get; set; } = new UserData();

        public string? Class { get; set; } = string.Empty;

        public List<UserData>? Watchers { get; set; } = new List<UserData>();

        public string? TicketType { get; set; } = string.Empty;

        public string? Priority { get; set; } = string.Empty;

        public Dictionary<string, List<string>>? CustomFields { get; set; } = new Dictionary<string, List<string>>();

        public List<string>? Labels { get; set; } = new List<string>();

        public string Status { get; set; } = string.Empty;
    }
}
