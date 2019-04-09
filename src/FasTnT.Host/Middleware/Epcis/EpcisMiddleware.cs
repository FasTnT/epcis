using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using FasTnT.Model.Responses;
using FasTnT.Domain;
using System.Threading;

namespace FasTnT.Host
{
    public abstract class EpcisMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private readonly string _path;
        private IServiceProvider _serviceProvider;
        private HttpContext _httpContext;

        public EpcisMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _path = path;
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            if (httpContext.Request.Method == "POST" && httpContext.Request.Path.StartsWithSegments(_path))
            {
                _serviceProvider = serviceProvider;
                _httpContext = httpContext;

                var formatterFactory = serviceProvider.GetService<FormatterProvider>();
                var contentType = _httpContext.Request.ContentType;
                var request = await formatterFactory.GetFormatter<T>(contentType).Read(httpContext.Request.Body, httpContext.RequestAborted);

                await Process(request, httpContext.RequestAborted);
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        public abstract Task Process(T request, CancellationToken cancellationToken);

        public async Task Execute<TService>(Func<TService, Task> action) => await action(_serviceProvider.GetService<TService>());

        public async Task Execute<TService>(Func<TService, Task<IEpcisResponse>> action, CancellationToken cancellationToken)
        {
            var result = await action(_serviceProvider.GetService<TService>());
            await _httpContext.SetEpcisResponse(result, cancellationToken);
        }
    }
}
