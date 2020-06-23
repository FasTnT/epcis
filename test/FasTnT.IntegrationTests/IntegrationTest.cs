using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System.Net.Http;

namespace FasTnT.IntegrationTests
{
    [TestClass]
    public static class IntegrationTest
    {
        private static TestServer TestServer { get; set; }
        public static IConfiguration Configuration { get; private set; }
        public static HttpClient Client { get; private set; }

        [AssemblyInitialize]
        public static void IntegrationTestInitialize(TestContext context)
        {
            Configuration = new ConfigurationBuilder().AddUserSecrets<TestStartup>().Build();

            var host = new WebHostBuilder().UseEnvironment("Development")
                          .UseIISIntegration()
                          .UseConfiguration(Configuration)
                          .UseStartup<TestStartup>();

            EnsureConnectionStringIsSpecified(context);

            TestServer = new TestServer(host);

            Client = TestServer.CreateClient();
        }

        [AssemblyCleanup]
        public static void IntegrationTestCleanup()
        {
            if (Client != null) Client.Dispose();
            if (TestServer != null) TestServer.Dispose();
        }

        private static void EnsureConnectionStringIsSpecified(TestContext context)
        {
            var connectionString = Configuration.GetConnectionString("FasTnT.Database");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                context.WriteLine("No connection string is setup for integration tests. Stopping execution.");
                throw new AssertInconclusiveException("No connection string is setup for integration tests. Stopping execution.");
            }
            else if (!CanConnectToDatabase(connectionString))
            {
                context.WriteLine("Invalid connection string is setup for integration tests. Stopping execution.");
                throw new AssertInconclusiveException("Invalid connection string is setup for integration tests. Stopping execution.");
            }
        }

        private static bool CanConnectToDatabase(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
