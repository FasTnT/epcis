using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FasTnT.Host.Middleware
{
    public class HttpSynchronousIOMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpSynchronousIOMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            var bodyControlFeature = context.Features.Get<IHttpBodyControlFeature>();

            if(bodyControlFeature != null)
            {
                bodyControlFeature.AllowSynchronousIO = true;
            }

            await _next(context);
        }
    }
}
