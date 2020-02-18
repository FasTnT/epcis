using FasTnT.Commands.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    [TestClass]
    public class WhenFormattingAGetSubscriptionIDsResponse : XmlFormatterTestBase
    {
        public override void Given()
        {
            base.Given();

            Response = new GetSubscriptionIdsResponse { SubscriptionIds = new[] { "test-subscription-1" } };
        }

        [TestMethod]
        public void ItShouldReturnAValidXmlDocument()
        {
            try
            {
                var doc = XDocument.Parse(Formatted);
                Assert.IsNotNull(doc);
                Assert.AreEqual(XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1"), doc.Root.Name);
            }
            catch
            {
                Assert.Fail("Formatted is not a valid XML");
            }
        }

        [TestMethod]
        public void ItShouldContainAGetSubscriptionIDsResultWithTheCorrectVersion()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("GetSubscriptionIDsResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsNotNull(element);
            Assert.IsTrue(element.Value.Contains("test-subscription-1"), "Response should contain the subscription IDs");
        }
    }
}
