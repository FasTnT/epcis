using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Services.Dispatch;
using FasTnT.Domain.Services.Handlers;
using FasTnT.Domain.Services.Handlers.Queries;
using FasTnT.Domain.Services.Handlers.Subscriptions;
using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Model.Queries.Implementations;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using System;
using System.Linq;
using System.Reflection;

namespace FasTnT.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEpcisDomain(this IServiceCollection services)
        {
            var handlers = Assembly.GetAssembly(typeof(Dispatcher)).ExportedTypes.Where(x =>
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<>)) ||
                x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriptionHandler<>))).ToArray();

            var queries = Assembly.GetAssembly(typeof(Dispatcher)).ExportedTypes.Where(x => x.GetInterfaces().Any(i => i == typeof(IEpcisQuery))).Select(x => Activator.CreateInstance(x)).Cast<IEpcisQuery>().ToArray();

            handlers.ForEach(x => services.AddScoped(x));
            services.AddScoped(typeof(IDispatcher), typeof(Dispatcher));
            services.AddScoped(typeof(ISubscriptionResultSender), typeof(HttpSubscriptionResultSender));
            services.AddScoped(typeof(SubscriptionRunner));
            services.AddSingleton(typeof(Type[]), handlers);
            services.AddSingleton(typeof(IEpcisQuery[]), queries);
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionBackgroundService>();
        }
    }
}
