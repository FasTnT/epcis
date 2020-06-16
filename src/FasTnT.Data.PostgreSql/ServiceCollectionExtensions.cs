using Dapper;
using FasTnT.Data.PostgreSql.Query;
using FasTnT.Data.PostgreSql.Migrations;
using FasTnT.Data.PostgreSql.Subscriptions;
using FasTnT.Domain.Data;
using FasTnT.PostgreSql.Capture;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace FasTnT.Data.PostgreSql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEpcisPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IEpcisRequestStore, EpcisRequestStore>();
            services.AddScoped<IEventFetcher, EventFetcher>();
            services.AddScoped<IMasterdataFetcher, MasterdataFetcher>();
            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped(s => OpenConnection(connectionString));

            DefaultTypeMap.MatchNamesWithUnderscores = true;
            DatabaseMigrator.Migrate(connectionString);

            return services;
        }

        private static IDbConnection OpenConnection(string connectionString)
        {
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            return conn;
        }
    }
}
