using FasTnT.Model.Users;

namespace FasTnT.Domain.Commands.Responses
{
    public class UserLogInResponse
    {
        public bool Authorized { get; set; }
        public User User { get; set; }
    }
}
