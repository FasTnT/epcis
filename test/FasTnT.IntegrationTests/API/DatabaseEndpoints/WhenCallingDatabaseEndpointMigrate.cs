using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;

namespace FasTnT.IntegrationTests.API.DatabaseEndpoints
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenCallingDatabaseEndpointMigrate : BaseIntegrationTest
    {
        public override void Act()
        {
            Result = Client.PostAsync("/Setup/Database/Migrate", null).Result;
        }

        [Assert]
        public void ItShouldReturnHttp200OKContent() => Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);

        [Assert]
        public void ItShouldNotReturnAnyContent() => Assert.AreEqual(string.Empty, Result.Content.ReadAsStringAsync().Result);

        [Assert]
        public void TheDatabaseShouldContainAllFasTnTSchemas()
        {
            var schemaNames = GetDatabaseSchemaNames();
            Assert.IsTrue(new[] { "users", "epcis", "cbv", "subscriptions", "callback", "sbdh" }.All(x => schemaNames.Any(s => s == x)), "A FasTnT schema has not been created");
        }

        private string[] GetDatabaseSchemaNames() => Query("SELECT schema_name FROM information_schema.schemata").ToArray();
    }
}
