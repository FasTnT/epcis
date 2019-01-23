using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FasTnT.Host.Infrastructure.Authentication
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string DefaultScheme = "BasicAuthentication";
        public const string Realm = "FasTnT";

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserContext _userContext;

        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUnitOfWork unitOfWork, UserContext userContext): base(options, logger, encoder, clock)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                _userContext.Authenticate(await _unitOfWork.UserManager.GetByUsername(username), password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header", new AuthenticationProperties { });
            }

            if (_userContext.Current == null) return AuthenticateResult.Fail("Invalid Username or Password");

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, _userContext.Current.UserName) }, Scheme.Name));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
