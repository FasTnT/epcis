using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.Database
{
    [TestClass]
    public class WhenCallingDatabaseEndpointMigrate : BaseIntegrationTest
    {
        public override void Arrange()
        {
            base.Arrange();
            Task.WaitAll(Client.PostAsync("/EpcisServices/1.2/Database/Rollback", null));
        }

        public override void Act()
        {
            Result = Client.PostAsync("/EpcisServices/1.2/Database/Migrate", null).Result;
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
        public void TheDatabaseShouldContainAllFasTnTSchemas()
        {
            var schemaNames = GetDatabaseSchemaNames();
            Assert.IsTrue(new[] { "users", "epcis", "cbv", "subscriptions", "callback" }.All(x => schemaNames.Any(s => s == x)), "A FasTnT schema has not been created");
        }

        private string[] GetDatabaseSchemaNames()
        {
            return Query("SELECT schema_name FROM information_schema.schemata").ToArray();
        }
    }
}
