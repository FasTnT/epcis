using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.XML.ListSubscriptionIDs
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenSendingASubscribeRequest : BaseMigratedIntegrationTest
    {
        public override void Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            Result = Client.PostAsync("/v1_2/Query", new StringContent(File.ReadAllText("Requests/XML/Subscribe.xml"), Encoding.UTF8, "application/xml")).Result;
        }

        [Assert]
        public void ItShouldReturnHttp200OK() => Assert.AreEqual(HttpStatusCode.NoContent, Result.StatusCode);

        [Assert]
        public void ItShouldReturnANullContent()
        {
            var content = Result.Content.ReadAsStringAsync().Result;

            Assert.IsTrue(string.IsNullOrEmpty(content), content);
        }
    }
}
