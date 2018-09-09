using FasTnT.Domain.Services.Dispatch;
using FasTnT.Domain.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Linq;
using System.Reflection;

namespace FasTnT.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEpcisDomain(this IServiceCollection services)
        {
            var handlers = Assembly.GetAssembly(typeof(Dispatcher)).ExportedTypes.Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<>))).ToArray();

            handlers.ForEach(x => services.AddScoped(x));
            services.AddScoped(typeof(IDispatcher), typeof(Dispatcher));
            services.AddSingleton(typeof(Type[]), handlers);
        }
    }
}
