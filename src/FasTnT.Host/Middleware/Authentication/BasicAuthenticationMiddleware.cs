using FasTnT.Domain.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Domain.Services.Users;
using System.Text;

namespace FasTnT.Host.Middleware.Authentication
{
    public class BasicAuthenticationMiddleware
    {
        const string AuthorizationHeaderKey = "Authorization";
        const string AuthenticateHeader = "WWW-Authenticate";
        const string AuthenticationScheme = "Basic";

        private readonly RequestDelegate _next;
        private readonly string _realm;

        public BasicAuthenticationMiddleware(RequestDelegate next, string realm)
        {
            _next = next;
            _realm = realm;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var authHeader = httpContext.Request.Headers.FirstOrDefault(x => x.Key == AuthorizationHeaderKey);

            if(!httpContext.Request.Headers.Any(x => x.Key == AuthorizationHeaderKey) || !authHeader.Value.FirstOrDefault().StartsWith($"{AuthenticationScheme} "))
            {
                Unauthenticated(httpContext);
            }
            else
            {
                var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
                var userContext = serviceProvider.GetService<UserContext>();
                var (username, password) = ParseCredentials(authHeader.Value.First());
                var user = await unitOfWork.UserManager.GetByUsername(username);

                if (userContext.Authenticate(user, password)) await _next(httpContext);
                else Unauthenticated(httpContext);
            }
        }

        private (string username, string password) ParseCredentials(string basicAuth)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(basicAuth.Substring($"{AuthenticationScheme} ".Length))).Split(':');

            return (credentials[0], credentials[1]);
        }

        private void Unauthenticated(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Add(AuthenticateHeader, $"{AuthenticationScheme} realm =\"{_realm}\"");
        }
    }
}
