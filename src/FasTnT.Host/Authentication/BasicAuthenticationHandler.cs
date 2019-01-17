using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FasTnT.Host.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await Task.Run(() =>
            {
                if (!Request.Headers.ContainsKey("Authorization")) return AuthenticateResult.Fail("Missing Authorization Header");

                var user = default(string);
                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];

                    if (username == "Admin" && password == "P@ssw0rd")
                    {
                        user = username;
                    }
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }

                if (user == null) return AuthenticateResult.Fail("Invalid Username or Password");

                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user), new Claim(ClaimTypes.Name, user) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            });
        }
    }
}
