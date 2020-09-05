using FasTnT.Commands.Responses;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.MasterDatas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.IntegrationTests.Formatters.XML
{
    [TestClass]
    public class WhenFormattingAPollResponse : XmlFormatterTestBase
    {
        public override void Given()
        {
            base.Given();

            Response = new PollResponse { EventList = new List<EpcisEvent> { new EpcisEvent { Type = EventType.Object, Action = EventAction.Add, Epcs = new List<Epc>{ new Epc { Type = EpcType.List, Id = "123456789" } } } }, MasterdataList = new List<EpcisMasterData> { new EpcisMasterData { Id = "testMD", Type = "cbv:test:masterdata" } }, QueryName = "TestQuery", SubscriptionId = "TestSubscription" };
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
        public void ItShouldContainTheCorrectEvents()
        {
            var element = XDocument.Parse(Formatted).Root.Element("EPCISBody").Element(XName.Get("QueryResults", "urn:epcglobal:epcis-query:xsd:1"));
            var eventList = element.Element("resultsBody").Element("EventList");

            Assert.AreEqual(1, eventList.Elements().Count());
            Assert.AreEqual("ObjectEvent", eventList.Elements().First().Name.LocalName);
        }
    }
}
