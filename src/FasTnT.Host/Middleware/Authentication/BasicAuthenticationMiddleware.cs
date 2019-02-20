using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Host.Middleware.Authentication
{
    public class BasicAuthenticationMiddleware
    {
        private readonly ILogger<BasicAuthenticationMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly string _realm;

        public BasicAuthenticationMiddleware(ILogger<BasicAuthenticationMiddleware> logger, RequestDelegate next, string realm)
        {
            _logger = logger;
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
                // TODO: validate and authenticate user

                await _next(httpContext);
            }
        }

        private void Unauthenticated(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", _realm));
        }
    }
}
