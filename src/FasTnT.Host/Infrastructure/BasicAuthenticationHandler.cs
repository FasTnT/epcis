using FasTnT.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FasTnT.Domain.Commands.Requests;

namespace FasTnT.Host
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string HeaderKey = "Authorization";

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
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
                var (username, password) = ParseAuthenticationHeader(authHeader);

                return await AuthenticateUser(username, password);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private (string username, string password) ParseAuthenticationHeader(AuthenticationHeaderValue authHeader)
        {
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

            return credentials.Length == 2
                ? (credentials[0], credentials[1])
                : throw new FormatException($"{HeaderKey} header must contain 2 values separated by ':'");
        }

        private async Task<AuthenticateResult> AuthenticateUser(string username, string password)
        {
            var context = Request.HttpContext.RequestServices.GetService<RequestContext>();
            var mediator = Request.HttpContext.RequestServices.GetService<IMediator>();
            var response = await mediator.Send(new UserLogInRequest { Username = username, Password = password });

            if (response.Authorized)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username),
                };
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                context.User = response.User;

                return AuthenticateResult.Success(ticket);
            }
            else
            {
                return AuthenticateResult.Fail($"Invalid {HeaderKey} Header");
            }
        }
    }
}
