using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1._2.XML.Capture
{
    [TestClass]
    public class WhenCapturingAnEvent : BaseIntegrationTest
    {
        public override async Task Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            Result = await Client.PostAsync("/v1_2/Capture", new StringContent(File.ReadAllText("Requests/Capture/Capture.xml"), Encoding.UTF8, "application/xml"));
        }

        [Assert]
        public void ItShouldReturnHttp200OKContent() => Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);

        [Assert]
        public async Task ItShouldReturnANullContent()
        {
            var content = await Result.Content.ReadAsStringAsync();

            Assert.IsTrue(string.IsNullOrEmpty(content));
        }
    }
}
