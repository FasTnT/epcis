using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.Database
{
    [TestClass]
    public class WhenCallingDatabaseEndpointMigrate : BaseIntegrationTest
    {
        public override void Act()
        {
            Result = Client.PostAsync("/EpcisServices/1.2/Database/Migrate", null).Result;
        }

        [Assert]
        public void ItShouldReturnHttp200OK()
        {
            Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);
        }
    }
}
