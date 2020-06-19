using FasTnT.IntegrationTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.API.EpcisEndpoints.v1_2.Soap.GetStandardVersion
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class WhenCallingQueryGetStandardVersion : BaseIntegrationTest
    {
        public override void Act()
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");
            Result = Client.PostAsync("/v1_2/Query.svc", new StringContent(File.ReadAllText("Requests/Queries/GetStandardVersion.xml"), Encoding.UTF8, "application/xml")).Result;
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
            var standardVersion = xmlDocument.Root.Element(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/")).Element(XName.Get("GetStandardVersionResult", "urn:epcglobal:epcis-query:xsd:1")).Value;

            Assert.AreEqual("1.2", standardVersion);
        }
    }
}
