using Dapper;
using FasTnT.Domain.Persistence;
using FasTnT.Model.Events.Enums;
using FasTnT.Persistence.Dapper.DapperConfiguration;
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
            SqlMapper.AddTypeHandler(ArrayOfEnumerationHandler<EpcType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EventAction>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<FieldType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<SourceDestinationType>.Default);
            SqlMapper.AddTypeHandler(EnumerationHandler<EpcType>.Default);
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddScoped(typeof(IDbConnection), ctx => new NpgsqlConnection(connectionString));
            services.AddScoped(typeof(IUnitOfWork), typeof(DapperUnitOfWork));
        }
    }
}
