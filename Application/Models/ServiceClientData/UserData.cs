﻿
namespace BocchiTracker.ServiceClientData
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

        public static UserData sUnknown
        { 
            get 
            {
                return new UserData { 
                    Name    = string.Empty, 
                    Email   = string.Empty, 
                    Id      = "-1", 
                    IconURL = string.Empty };
            } 
        }

        public bool Contains(string inValue)
        {
            string v = inValue.ToLower();
            if (Name != null && Name.ToLower().Contains(inValue))
                return true;

            if (Email != null && Email.ToLower().Contains(inValue))
                return true;

            if (Id != null && Id.ToLower().Contains(inValue))
                return true;

            return false;
        }
    }
}
