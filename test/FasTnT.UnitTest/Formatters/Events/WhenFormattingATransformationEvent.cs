using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Formatters.Events
{
    [TestClass]
    public class WhenFormattingATransformationEvent : EventFormattingTestBase
    {
        public override void Given()
        {
            Event = new EpcisEvent
            {
                Type = EventType.Transformation,
                BusinessLocation = "test:location",
                BusinessStep = "test:step",
                CaptureTime = new System.DateTime(2020, 05, 15, 12, 32, 32),
                EventTime = new System.DateTime(2020, 05, 15, 13, 00, 00),
                EventTimeZoneOffset = new TimeZoneOffset { Value = 60 },
                SourceDestinationList = new List<SourceDestination> { new SourceDestination { Direction = SourceDestinationType.Source, Id = "source_id", Type = "source" } },
                Epcs = new List<Epc> { new Epc { Type = EpcType.OutputQuantity, Id = "epc:1", IsQuantity = true, Quantity = 25, UnitOfMeasure = "KGM" } },
                CustomFields = new List<CustomField> { new CustomField { Type = FieldType.CustomField, Name = "test", Namespace = "ns_test", Children = new List<CustomField> { new CustomField { Type = FieldType.CustomField, Name = "inner", Namespace = "ns_test", TextValue = "value" } } } }
            };

            Event.SourceDestinationList.Add(new SourceDestination { Id = "test:source", Type = "source:type", Direction = SourceDestinationType.Source });
        }

        [TestMethod]
        public void ItShouldReturnATransformationEventInAnExtensionObject()
        {
            Assert.AreEqual("extension", Result.Name.LocalName);
            Assert.AreEqual("TransformationEvent", Result.Elements().First().Name.LocalName);
        }

        [TestMethod]
        public void TheTransformationEventShouldHaveABusinessLocationField()
        {
            Assert.IsNotNull(Result.Elements().First().Element("bizLocation"));
            Assert.AreEqual("test:location", Result.Elements().First().Element("bizLocation").Element("id").Value);
        }

        [TestMethod]
        public void TheTransformationEventShouldHaveAnOutputQuantityListField()
        {
            Assert.IsNotNull(Result.Elements().First().Element("outputQuantityList"));
            Assert.AreEqual(1, Result.Elements().First().Element("outputQuantityList").Elements("quantityElement").Count());
        }

        [TestMethod]
        public void TheTransformationEventShouldHaveACustomField()
        {
            Assert.IsNotNull(Result.Elements().First().Element(XName.Get("test", "ns_test")));
        }

        [TestMethod]
        public void TheTransformationEventShouldHaveANestedCustomField()
        {
            Assert.IsNotNull(Result.Elements().First().Element(XName.Get("test", "ns_test")).Element(XName.Get("inner", "ns_test")));
            Assert.AreEqual("value", Result.Elements().First().Element(XName.Get("test", "ns_test")).Element(XName.Get("inner", "ns_test")).Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTime()
        {
            Assert.IsNotNull(Result.Elements().First().Element("eventTime"));
            Assert.AreEqual("2020-05-15T13:00:00Z", Result.Elements().First().Element("eventTime").Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTimeZoneOffset()
        {
            Assert.IsNotNull(Result.Elements().First().Element("eventTimeZoneOffset"));
            Assert.AreEqual("+01:00", Result.Elements().First().Element("eventTimeZoneOffset").Value);
        }
    }
}
