using FasTnT.Model.Utils;
using System;

namespace FasTnT.Model.Users
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public UserType Type { get; set; }
        public int RemainingRequests { get; set; } = 1;
        public TimeSpan LimitResetTimeout { get; set; }
    }

    public class UserType : Enumeration
    {
        public static UserType Admin = new UserType(0, "Admin");
        public static UserType RestrictedUser = new UserType(1, "Restricted user");

        public UserType() { }
        public UserType(short id, string displayName) : base(id, displayName) { }
    }
}
