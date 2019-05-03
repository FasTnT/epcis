using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.GetStandardVersion
{
    [TestClass]
    public class WhenCallingQueryGetStandardVersion : BaseIntegrationTest
    {
        public override void Act()
        {
            Task.WaitAll(Client.PostAsync("/EpcisServices/1.2/Database/Migrate", null));

            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            Result = Client.PostAsync("/EpcisServices/1.2/Query", new StringContent(File.ReadAllText("Requests/GetStandardVersion.xml"), Encoding.UTF8, "application/xml")).Result;
        }

        [Assert]
        public void ItShouldReturnHttp200OK()
        {
            Assert.AreEqual(HttpStatusCode.OK, Result.StatusCode);
        }

        [Assert]
        public void ItShouldReturnANotNullContent()
        {
            var content = Result.Content.ReadAsStringAsync().Result;

            Assert.IsNotNull(content);
        }

        [Assert]
        public void ItShouldReturnAValidXmlContent()
        {
            var content = Result.Content.ReadAsStringAsync().Result;
            var xmlDocument = XDocument.Parse(content);

            Assert.IsNotNull(xmlDocument);
        }

        [Assert]
        public void ItShouldReturnTheStandardVersion1_2()
        {
            var content = Result.Content.ReadAsStringAsync().Result;
            var xmlDocument = XDocument.Parse(content);
            var standardVersion = xmlDocument.Root.Element("EPCISBody").Element(XName.Get("GetStandardVersionResult", "urn:epcglobal:epcis-query:xsd:1")).Value;

            Assert.AreEqual("1.2", standardVersion);
        }
    }
}
