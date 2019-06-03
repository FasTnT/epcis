using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host.Middleware
{
    public class HttpStatusCodeHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpStatusCodeHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            
            if(!context.Response.HasStarted && context.Response.StatusCode != 401) context.Response.StatusCode = StatusCodes.Status204NoContent;
        }
    }
}
