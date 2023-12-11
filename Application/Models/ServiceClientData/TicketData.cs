using System;
using System.Collections.Generic;

namespace BocchiTracker.ServiceClientData
{
    [Flags]
    public enum TicketFilter
    {
        Id          = 1,
        Summary     = 2,
        Description = 4,
        Assign      = 8,
        Class       = 16,
        Priority    = 32,
        Labels      = 64,
        Status      = 128,
    }

    public class TicketData
    {
        public ServiceDefinitions Service { get; set; }

        public string Id { get; set; } = string.Empty;

        public string? Summary { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public UserData? Assign { get; set; } = new UserData();

        public string? Class { get; set; } = string.Empty;

        public List<UserData>? Watchers { get; set; } = new List<UserData>();

        public string? TicketType { get; set; } = string.Empty;

        public string? Priority { get; set; } = string.Empty;

        public CustomFields CustomFields { get; set; } = new CustomFields();

        public List<string>? Labels { get; set; } = new List<string>();

        public string Status { get; set; } = string.Empty;

        public bool Contatins(TicketFilter inFilter, string? inValue)
        {
            if (string.IsNullOrEmpty(inValue))
                return true;

            string v = inValue.ToLower();

            if (inFilter.HasFlag(TicketFilter.Id) && Id.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Summary) && Summary != null && Summary.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Description) && Description != null && Description.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Assign) && Assign != null && Assign.Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Class) && Class != null && Class.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Priority) && Priority != null && Priority.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Status) && Status.ToLower().Contains(v))
                return true;

            if (inFilter.HasFlag(TicketFilter.Labels))
            {
                foreach(var label in Labels)
                {
                    if (label.ToLower().Contains(v))
                        return true;
                }
            }

            return false;
        }

        public bool CanJumpPlayer()
        {
            if (CustomFields == null)
                return false;

            List<bool> validValues = new List<bool>();
            if (CustomFields.TryGetValue("PlayerPosition.x", out float outX))
                validValues.Add(true);


            if (CustomFields.TryGetValue("PlayerPosition.y", out float outY))
                validValues.Add(true);

            if (CustomFields.TryGetValue("PlayerPosition.z", out float outZ))
                validValues.Add(true);

            return validValues.Count >= 2;
        }
    }
}
