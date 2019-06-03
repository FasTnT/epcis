using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;

namespace FasTnT.IntegrationTests.API.DatabaseEndpoints
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenCallingDatabaseEndpointRollback : BaseMigratedIntegrationTest
    {
        public override void Act()
        {
            Result = Client.PostAsync("/Setup/Database/Rollback", null).Result;
        }

        [Assert]
        public void ItShouldReturnHttp204NoContent() => Assert.AreEqual(HttpStatusCode.NoContent, Result.StatusCode);

        [Assert]
        public void ItShouldNotReturnAnyContent() => Assert.AreEqual(string.Empty, Result.Content.ReadAsStringAsync().Result);

        [Assert]
        public void TheDatabaseShouldNotContainFasTnTSchema()
        {
            var schemaNames = GetDatabaseSchemaNames();
            Assert.IsTrue(!schemaNames.Any(x => new[] { "users", "epcis", "cbv", "subscriptions", "callback", "sbdh" }.Contains(x)), "A schema from FasTnT exists and should not.");
        }

        private string[] GetDatabaseSchemaNames() => Query("SELECT schema_name FROM information_schema.schemata").ToArray();
    }
}
