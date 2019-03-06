using Microsoft.AspNetCore.Builder;

namespace FasTnT.Host.Middleware.Authentication
{
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder applicationBuilder, string realm = "FasTnT")
            => applicationBuilder.UseMiddleware<BasicAuthenticationMiddleware>(realm);
    }
}
