using FasTnT.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using FasTnT.Model.Subscriptions;

namespace FasTnT.Host
{
    internal class EpcisSubscriptionTriggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;

        public EpcisSubscriptionTriggerMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _path = $"{path.TrimEnd('/')}/";
        }

        public async Task Invoke(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            if (httpContext.Request.Method == "GET" && httpContext.Request.Path.Value.StartsWith(_path) && !httpContext.Request.Path.Value.EndsWith('/'))
            {
                var service = serviceProvider.GetService<SubscriptionService>();
                var triggerName = httpContext.Request.Path.Value.Split('/').Last();

                await service.Process(new TriggerSubscriptionRequest { Trigger = triggerName });
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
