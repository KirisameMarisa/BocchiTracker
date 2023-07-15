
using System.Runtime.CompilerServices;

namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class UserData
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Id { get; set; }

        public string? IconURL { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }

        public override bool Equals(object? obj)
        {
            var x = obj as UserData;
            if (x == null) return false;
            return x.Email == Email;
        }

        public override int GetHashCode()
        {
            if (Email == null) return 0;
            return Email.GetHashCode();
        }
    }
}
