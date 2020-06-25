using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingXmlGetSubscriptionIDsRequest : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:epcglobal:epcis-query:xsd:1\"><soapenv:Body><urn:GetSubscriptionIDs><queryName>SimpleEventQuery</queryName></urn:GetSubscriptionIDs></soapenv:Body></soapenv:Envelope>");
        }

        [TestMethod]
        public void ItShouldReturnAGetSubscriptionIDsRequest()
        {
            Assert.IsInstanceOfType(Result, typeof(GetSubscriptionIdsRequest));
        }

        [TestMethod]
        public void ItShouldContainTheExpectedQueryName()
        {
            var getSubscriptionIdsRequest = (GetSubscriptionIdsRequest) Result;

            Assert.AreEqual("SimpleEventQuery", getSubscriptionIdsRequest.QueryName);
        }
    }
}
