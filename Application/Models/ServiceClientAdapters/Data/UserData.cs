
namespace BocchiTracker.ServiceClientAdapters.Data
{
    public class UserData
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Id { get; set; }

        public string? IconURL { get; set; }

        public override bool Equals(object? obj)
        {
            var x = obj as UserData;
            if (x == null) return false;
            return x.Name == Name;
        }

        public override int GetHashCode()
        {
            if (Name == null) return 0;
            return Name.GetHashCode();
        }
    }
}
