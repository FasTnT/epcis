using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using FasTnT.Data.PostgreSql.Migration;

namespace FasTnT.IntegrationTests.Common
{
    public abstract class BaseIntegrationTest
    {
        public HttpClient Client { get; private set; }
        public IDbConnection Connection { get; private set; }
        public HttpResponseMessage Result { get; set; }

        public virtual void Arrange()
        {
            Client = IntegrationTest.Client;
            Connection = new NpgsqlConnection(IntegrationTest.Configuration.GetConnectionString("FasTnT.Database"));
            Connection.Open();
        }

        public abstract void Act();

        [TestInitialize]
        public void Execute()
        {
            DatabaseMigrator.Migrate(IntegrationTest.Configuration.GetConnectionString("FasTnT.Database"));
            Arrange();
            Act();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"DROP SCHEMA sbdh CASCADE; DROP SCHEMA callback CASCADE; DROP SCHEMA subscriptions CASCADE; DROP SCHEMA cbv CASCADE; DROP SCHEMA epcis CASCADE; DROP SCHEMA users CASCADE; DELETE FROM public.schemaversions;";
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<string> Query(string sqlCommand)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = sqlCommand;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }
    }
}
