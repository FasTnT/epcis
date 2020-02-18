using FasTnT.Commands.Responses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    [TestClass]
    public class WhenFormattingAGetQueryNamesResponse : XmlFormatterTestBase
    {
        public override void Given()
        {
            base.Given();

            Response = new GetQueryNamesResponse { QueryNames = new[] { "SimpleEventQuery" } };
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
        public void ItShouldContainAGetQueryNamesResultWithTheCorrectVersion()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("GetQueryNamesResult", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsNotNull(element);
            Assert.IsTrue(element.Value.Contains("SimpleEventQuery"), "Response should contain the query names");
        }
    }
}
