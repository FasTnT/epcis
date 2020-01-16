using FasTnT.Domain.Commands.Responses;
using MediatR;

namespace FasTnT.Domain.Commands.Requests
{
    public class UserLogInRequest : IRequest<UserLogInResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
