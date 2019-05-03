using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.GetStandardVersion
{
    [TestClass]
    public class WhenCallingQueryGetStandardVersion : BaseIntegrationTest
    {
        public override void Act()
        {
            Task.WaitAll(Client.PostAsync("/EpcisServices/1.2/Database/Migrate", null));

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            Result = Client.PostAsync("/EpcisServices/1.2/Query", new StringContent("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><epcisq:EPCISQueryDocument xmlns:epcisq=\"urn:epcglobal:epcis-query:xsd:1\" schemaVersion=\"1\" creationDate=\"2018-02-07T11:52:42.685Z\"><EPCISBody><epcisq:GetStandardVersion /></EPCISBody></epcisq:EPCISQueryDocument>", Encoding.UTF8, "application/xml")).Result;;
        }

        [Assert]
        public void ItShouldReturnHttp200OK()
        {
            Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);
        }
    }
}
