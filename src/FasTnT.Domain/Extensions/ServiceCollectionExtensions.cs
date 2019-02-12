using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Services;
using FasTnT.Domain.Services.Subscriptions;
using FasTnT.Domain.Services.Users;
using FasTnT.Model.Queries.Implementations;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton(typeof(IEpcisQuery[]), queries);

            services.AddScoped(typeof(CaptureService));
            services.AddScoped(typeof(QueryService));
            services.AddScoped(typeof(SubscriptionService));
            services.AddScoped(typeof(CallbackService));
            services.AddScoped(typeof(UserContext));
            services.AddScoped(typeof(ISubscriptionResultSender), typeof(HttpSubscriptionResultSender));
            services.AddScoped(typeof(SubscriptionRunner));
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionBackgroundService>();
        }
    }
}
