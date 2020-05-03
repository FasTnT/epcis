using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1._2.XML.Invalid
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenCallingTheAPIUsingWrongCredentials : BaseIntegrationTest
    {
        public override void Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46SW52YWxpZFB3ZA==");
            Result = Client.PostAsync("/v1_2/Query", new StringContent(File.ReadAllText("Requests/XML/Poll.xml"), Encoding.UTF8, "application/xml")).Result;
        }

        [Assert]
        public void ItShouldReturnHttp401Unauthorized() => Assert.AreEqual(HttpStatusCode.Unauthorized, Result.StatusCode);
    }
}
