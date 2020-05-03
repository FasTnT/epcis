using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;

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
            Arrange();
            Act();
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
