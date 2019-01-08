using Dapper;
using FasTnT.Domain;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Domain.Services.Setup;
using FasTnT.Persistence.Dapper.DapperConfiguration;
using FasTnT.Persistence.Dapper.Setup;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace FasTnT.Persistence.Dapper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEpcisPersistence(this IServiceCollection services, string connectionString)
        {
            SqlMapper.AddTypeHandler(TimezoneOffsetHandler.Default);
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EventType>.Default);
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EventAction>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventAction>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<FieldType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<SourceDestinationType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EpcType>.Default);
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddScoped(typeof(IDbConnection), ctx => new NpgsqlConnection(connectionString));
            services.AddScoped(typeof(IUnitOfWork), typeof(DapperUnitOfWork));
            services.AddScoped(typeof(IEventStore), typeof(EventStore));
            services.AddScoped(typeof(IEventRepository), typeof(PgSqlEventRepository));
            services.AddScoped(typeof(IDatabaseMigrator), typeof(PgSqlDatabaseMigrator));
        }
    }
}
