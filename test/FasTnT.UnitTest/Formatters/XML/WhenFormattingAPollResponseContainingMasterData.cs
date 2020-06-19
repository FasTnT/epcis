using FasTnT.Commands.Responses;
using FasTnT.Model.MasterDatas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    [TestClass]
    public class WhenFormattingAPollResponseContainingMasterData : XmlFormatterTestBase
    {
        public override void Given()
        {
            base.Given();

            Response = new PollResponse { MasterdataList = new List<EpcisMasterData> { new EpcisMasterData { Id = "testMD", Type = "cbv:test:masterdata" } }, QueryName = "TestQuery", SubscriptionId = "TestSubscription" };
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
        public void ItShouldContainAQueryResultsWithCorrectQueryNameAndSubscriptionId()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1"));
            Assert.IsNotNull(element);
            Assert.AreEqual("TestQuery", element.Element("queryName").Value);
            Assert.AreEqual("TestSubscription", element.Element("subscriptionID").Value);
        }

        [TestMethod]
        public void ItShouldContainTheCorrectMasterData()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1"));
            var masterdataList = element.Element("resultsBody").Element("VocabularyList");

            Assert.AreEqual(1, masterdataList.Elements().Count());
            Assert.AreEqual("Vocabulary", masterdataList.Elements().First().Name.LocalName);
        }
    }
}
