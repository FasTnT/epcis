using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using FasTnT.Model.Responses;
using FasTnT.Domain;
using System.Threading;
using FasTnT.Model;

namespace FasTnT.Host
{
    public abstract class EpcisMiddleware<T> where T : IEpcisPayload
    {
        const int OkStatusCode = 200;

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
            if (HttpMethods.IsPost(httpContext.Request.Method) && httpContext.Request.Path.StartsWithSegments(_path))
            {
                _httpContext = httpContext;
                _serviceProvider = serviceProvider;

                await DispatchRequest(httpContext, serviceProvider);
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        private async Task DispatchRequest(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var formatterFactory = serviceProvider.GetService<FormatterProvider>();
            var request = await formatterFactory.GetFormatter<T>(_httpContext.Request.ContentType).Read(httpContext.Request.Body, httpContext.RequestAborted);

            await Process(request, httpContext.RequestAborted);
        }

        public abstract Task Process(T request, CancellationToken cancellationToken);
        public async Task Execute<TService>(Func<TService, Task> action) => await action(_serviceProvider.GetService<TService>());

        public async Task Execute<TService>(Func<TService, Task<IEpcisResponse>> action)
        {
            var result = await action(_serviceProvider.GetService<TService>());
            await _httpContext.SetEpcisResponse(result, OkStatusCode, default);
        }
    }
}
