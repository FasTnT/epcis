using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Formatters.Events
{
    [TestClass]
    public class WhenFormattingAnObjectEvent : EventFormattingTestBase
    {
        public override void Given()
        {
            Event = new EpcisEvent
            {
                Type = EventType.Object,
                Action = EventAction.Add,
                BusinessLocation = "test:location",
                BusinessStep = "test:step",
                CaptureTime = new System.DateTime(2020, 05, 15, 12, 32, 32),
                EventTime = new System.DateTime(2020, 05, 15, 13, 00, 00),
                EventTimeZoneOffset = new TimeZoneOffset { Value = 60 },
                SourceDestinationList = new List<SourceDestination> { new SourceDestination { Direction = SourceDestinationType.Source, Id = "source_id", Type = "source" } },
                Epcs = new List<Epc> { new Epc { Type = EpcType.List, Id = "epc:1" } },
                CustomFields = new List<CustomField> { 
                    new CustomField { Type = FieldType.Ilmd, Name = "test", Namespace = "ns_test", TextValue = "value" } 
                }
            };

            Event.SourceDestinationList.Add(new SourceDestination { Id = "test:source", Type = "source:type", Direction = SourceDestinationType.Source });
        }

        [TestMethod]
        public void ItShouldReturnAnObjectEvent()
        {
            Assert.AreEqual("ObjectEvent", Result.Name.LocalName);
        }

        [TestMethod]
        public void TheObjectEventShouldHaveAnActionField()
        {
            Assert.IsNotNull(Result.Element("action"));
            Assert.AreEqual("ADD", Result.Element("action").Value);
        }

        [TestMethod]
        public void TheObjectEventShouldHaveABusinessLocationField()
        {
            Assert.IsNotNull(Result.Element("bizLocation"));
            Assert.AreEqual("test:location", Result.Element("bizLocation").Element("id").Value);
        }

        [TestMethod]
        public void TheObjectEventShouldHaveAnEpcList()
        {
            Assert.IsNotNull(Result.Element("epcList"));
            Assert.AreEqual(1, Result.Element("epcList").Elements("epc").Count());
            Assert.AreEqual("epc:1", Result.Element("epcList").Elements("epc").First().Value);
        }

        [TestMethod]
        public void TheObjectEventShouldHaveAnIlmdField()
        {
            Assert.IsNotNull(Result.Element("extension").Element("ilmd"));
            Assert.AreEqual(1, Result.Element("extension").Element("ilmd").Elements().Count());
            Assert.AreEqual("value", Result.Element("extension").Element("ilmd").Element(XName.Get("test", "ns_test")).Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTime()
        {
            Assert.IsNotNull(Result.Element("eventTime"));
            Assert.AreEqual("2020-05-15T13:00:00Z", Result.Element("eventTime").Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTimeZoneOffset()
        {
            Assert.IsNotNull(Result.Element("eventTimeZoneOffset"));
            Assert.AreEqual("+01:00", Result.Element("eventTimeZoneOffset").Value);
        }
    }
}
