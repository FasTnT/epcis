using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FasTnT.Model.Users;

namespace FasTnT.Domain.Services.Users
{
    public class UserContext
    {
        static readonly SHA256 Sha256 = SHA256.Create();
        public User Current { get; private set; }

        public bool Authenticate(User user, string password)
        {
            if (user != null && VerifyPassword(user, password)) Current = user;

            return Current != null;
        }

        private bool VerifyPassword(User user, string password)
        {
            var hashed = string.Concat(Sha256.ComputeHash(Encoding.UTF8.GetBytes($"{user.UserName}_{password}")).Select(x => x.ToString("x2")));
            
            return hashed.Equals(user.Password, StringComparison.OrdinalIgnoreCase);
        }
    }
}
