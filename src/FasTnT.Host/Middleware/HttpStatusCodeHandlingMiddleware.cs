using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host.Middleware
{
    public class HttpStatusCodeHandlingMiddleware
    {
        const int NoContent = 204;
        private readonly RequestDelegate _next;

        public HttpStatusCodeHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            
            if(!context.Response.HasStarted) context.Response.StatusCode = NoContent;
        }
    }
}
