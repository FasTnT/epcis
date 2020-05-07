using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using FasTnT.Data.PostgreSql.Migrations;

namespace FasTnT.IntegrationTests.Common
{
    public abstract class BaseIntegrationTest
    {
        public string ConnectionString { get; private set; }
        public HttpClient Client { get; private set; }
        public IDbConnection Connection { get; private set; }
        public HttpResponseMessage Result { get; set; }

        public virtual void Arrange()
        {
            ConnectionString = IntegrationTest.Configuration.GetConnectionString("FasTnT.Database");
            Connection = new NpgsqlConnection(ConnectionString);
            Connection.Open();

            DatabaseMigrator.Migrate(ConnectionString);
            Client = IntegrationTest.Client;
        }

        public abstract void Act();

        [TestInitialize]
        public void Execute()
        {
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

            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                Connection.Dispose();
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
