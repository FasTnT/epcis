using FasTnT.Domain.BackgroundTasks;
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
            var queries = Assembly.GetAssembly(typeof(IEpcisQuery)).ExportedTypes.Where(x => x.GetInterfaces().Any(i => i == typeof(IEpcisQuery))).Select(x => Activator.CreateInstance(x)).Cast<IEpcisQuery>().ToArray();
            Assembly.GetAssembly(typeof(IEpcisQuery)).ExportedTypes.Where(x => x.Namespace.StartsWith("FasTnT.Domain.Services.Handlers")).ForEach(x => services.AddScoped(x));
            services.AddScoped(typeof(ISubscriptionResultSender), typeof(HttpSubscriptionResultSender));
            services.AddScoped(typeof(SubscriptionRunner));
            services.AddSingleton(typeof(IEpcisQuery[]), queries);
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionBackgroundService>();
        }
    }
}
