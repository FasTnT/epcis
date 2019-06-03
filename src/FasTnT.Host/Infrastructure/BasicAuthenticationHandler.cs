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

namespace FasTnT.Host
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string HeaderKey = "Authorization";
        private readonly UserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUnitOfWork unitOfWork, UserContext userContext)
            : base(options, logger, encoder, clock)
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderKey))
            {
                return AuthenticateResult.Fail($"Missing {HeaderKey} Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[HeaderKey]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                var user = await _unitOfWork.UserManager.GetByUsername(username, Request.HttpContext.RequestAborted);

                if (_userContext.Authenticate(user, password))
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                    };
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail($"Invalid {HeaderKey} Header");
                }
            }
            catch(Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }
    }
}
