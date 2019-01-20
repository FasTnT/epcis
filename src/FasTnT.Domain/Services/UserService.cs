using FasTnT.Model.Users;

namespace FasTnT.Domain.Persistence
{
    public class UserService
    {
        public User Current { get; private set; }

        public void Authenticate(string username, string password)
        {
            if (Equals(username, password))
            {
                Current = new User { UserName = username, Type = UserType.Admin };
            }
        }

        public bool CanMakeRequest(User user) => user != null && (user.Type == UserType.Admin || user.RemainingRequests > 0);
    }
}
