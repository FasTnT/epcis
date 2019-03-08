using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using FasTnT.Host.Infrastructure;
using FasTnT.Model.Responses;
using FasTnT.Domain;
using System.Linq;
using FasTnT.Formatters;

namespace FasTnT.Host
{
    public abstract class EpcisMiddleware<T>
    {
        private readonly RequestDelegate _next;
        private readonly string _path;
        private IServiceProvider _serviceProvider;
        private HttpContext _httpContext;

        public EpcisMiddleware(ILogger logger, RequestDelegate next, string path)
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
                var request = formatterFactory.GetFormatter<T>(contentType).Read(httpContext.Request.Body);

                await Process(request);
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        public abstract Task Process(T request);

        public async Task Execute<TService>(Func<TService, Task> action) => await action(_serviceProvider.GetService<TService>());

        public async Task Execute<TService>(Func<TService, Task<IEpcisResponse>> action)
        {
            var result = await action(_serviceProvider.GetService<TService>());

            var formatterFactory = _serviceProvider.GetService<FormatterProvider>();
            var contentType = GetContentType(_httpContext);
            var formatter = formatterFactory.GetFormatter<IEpcisResponse>(contentType) as IResponseFormatter;

            _httpContext.SetEpcisResponse(result, formatter);
        }

        private string GetContentType(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("Accept"))
            {
                var acceptValue = httpContext.Request.Headers["Accept"].First();
                if (acceptValue != "*/*") return acceptValue;
            }

            return httpContext.Request.ContentType;
        }
    }
}
