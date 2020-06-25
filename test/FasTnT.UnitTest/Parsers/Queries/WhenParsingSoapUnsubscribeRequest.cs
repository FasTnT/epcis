using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingSoapUnsubscribeRequest : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:epcglobal:epcis-query:xsd:1\"><soapenv:Body><urn:Unsubscribe><subscriptionID>TestSoapSubscription</subscriptionID></urn:Unsubscribe></soapenv:Body></soapenv:Envelope>");
        }

        [TestMethod]
        public void ItShouldReturnAnUnsubscribeRequest()
        {
            Assert.IsInstanceOfType(Result, typeof(UnsubscribeRequest));
        }

        [TestMethod]
        public void ItShouldHaveTheSpecifiedSubscriptionId()
        {
            var unsubscribe = (UnsubscribeRequest)Result;

            Assert.AreEqual("TestSoapSubscription", unsubscribe.SubscriptionId);
        }
    }
}
