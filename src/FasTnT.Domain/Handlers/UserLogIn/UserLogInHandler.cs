using FasTnT.Domain.Commands.Requests;
using FasTnT.Domain.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Model.Users;
using MediatR;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.UserLogIn
{
    public class UserLogInHandler : IRequestHandler<UserLogInRequest, UserLogInResponse>
    {
        private static readonly SHA256 Sha256 = SHA256.Create();
        private readonly IUserManager _userManager;

        public UserLogInHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserLogInResponse> Handle(UserLogInRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetByUsername(request.Username, cancellationToken);
            var response = new UserLogInResponse { User = user };

            if (user != null && VerifyPassword(user, request.Password))
            {
                response.Authorized = true;
            }

            return response;
        }

        private bool VerifyPassword(User user, string password)
        {
            var hashed = string.Concat(Sha256.ComputeHash(Encoding.UTF8.GetBytes($"{user.UserName}_{password}")).Select(x => x.ToString("x2")));

            return hashed.Equals(user.Password, StringComparison.OrdinalIgnoreCase);
        }
    }
}
