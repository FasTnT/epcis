using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.XML
{
    [TestClass]
    public class WhenParsingXmlUnsubscribeRequest : XmlParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><epcisq:EPCISQueryDocument xmlns:epcisq=\"urn:epcglobal:epcis-query:xsd:1\" creationDate=\"2019-01-26T20:10:01.8111457Z\" schemaVersion=\"1\"><EPCISBody><epcisq:Unsubscribe><subscriptionID>TestUnsubscription</subscriptionID></epcisq:Unsubscribe></EPCISBody></epcisq:EPCISQueryDocument>");
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

            Assert.AreEqual("TestUnsubscription", unsubscribe.SubscriptionId);
        }
    }
}
