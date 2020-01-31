using FasTnT.Commands.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    [TestClass]
    public class WhenFormattingAGetStandardVersionResponse : XmlFormatterTestBase
    {
        public override void Given()
        {
            base.Given();

            Response = new GetStandardVersionResponse { Version = "1.2" };
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
        public void ItShouldContainAGetStandardVersionResultWithTheCorrectVersion()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("GetStandardVersionResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsNotNull(element);
            Assert.AreEqual("1.2", element.Value);
        }
    }
}
