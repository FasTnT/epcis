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
        private readonly RequestDelegate _next;
        private readonly string _realm;

        public BasicAuthenticationMiddleware(RequestDelegate next, string realm)
        {
            _next = next;
            _realm = realm;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var authHeader = httpContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");

            if(!httpContext.Request.Headers.Any(x => x.Key == "Authorization") || !authHeader.Value.FirstOrDefault().StartsWith("Basic "))
            {
                Unauthenticated(httpContext);
            }
            else
            {
                var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
                var userContext = serviceProvider.GetService<UserContext>();
                var (username, password) = ParseCredentials(authHeader.Value.First());
                var user = await unitOfWork.UserManager.GetByUsername(username, httpContext.RequestAborted);

                if (userContext.Authenticate(user, password))
                {
                    await _next(httpContext);
                }
                else
                {
                    Unauthenticated(httpContext);
                }
            }
        }

        private (string username, string password) ParseCredentials(string basicAuth)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(basicAuth.Split(' ', 2).Last())).Split(':');

            return (credentials[0], credentials[1]);
        }

        private void Unauthenticated(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", _realm));
        }
    }
}
