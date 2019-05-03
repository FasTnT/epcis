using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;

namespace FasTnT.IntegrationTests.API.DatabaseEndpoints
{
    [TestClass]
    public class WhenCallingDatabaseEndpointRollback : BaseIntegrationTest
    {
        public override void Act()
        {
            Result = Client.PostAsync("/EpcisServices/1.2/Database/Rollback", null).Result;
        }

        [Assert]
        public void ItShouldReturnHttp200OK()
        {
            Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);
        }

        [Assert]
        public void ItShouldNotReturnAnyContent()
        {
            Assert.AreEqual(string.Empty, Result.Content.ReadAsStringAsync().Result);
        }

        [Assert]
        public void TheDatabaseShouldNotContainFasTnTSchema()
        {
            var schemaNames = GetDatabaseSchemaNames();
            Assert.IsTrue(!schemaNames.Any(x => new[] { "users", "epcis", "cbv", "subscriptions", "callback" }.Contains(x)), "A schema from FasTnT exists and should not.");
        }

        private string[] GetDatabaseSchemaNames()
        {
            return Query("SELECT schema_name FROM information_schema.schemata").ToArray();
        }
    }
}
