using Dapper;
using FasTnT.Data.PostgreSql.DapperConfiguration;
using FasTnT.Data.PostgreSql.DataRetrieval;
using FasTnT.Data.PostgreSql.Subscriptions;
using FasTnT.Data.PostgreSql.Users;
using FasTnT.Domain.Data;
using FasTnT.Model.Events.Enums;
using FasTnT.PostgreSql.Capture;
using FasTnT.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace FasTnT.Data.PostgreSql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEpcisPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDocumentStore, DocumentStore>();
            services.AddScoped<IDatabaseMigrator, DatabaseMigrator>();
            services.AddScoped<IEventFetcher, EventFetcher>();
            services.AddScoped<IMasterdataFetcher, MasterdataFetcher>();
            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped(s => OpenConnection(connectionString));


            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(TimezoneOffsetHandler.Default);
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EventType>.Default);
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EventAction>.Default);
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EpcType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventAction>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<FieldType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<SourceDestinationType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EpcType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<QueryCallbackType>.Default);
            //SqlMapper.AddTypeHandler(EnumerationHandler<SubscriptionResult>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<ContactInformationType>.Default);

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
