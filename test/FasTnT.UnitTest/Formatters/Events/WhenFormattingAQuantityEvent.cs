using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Formatters.Events
{
    [TestClass]
    public class WhenFormattingAQuantityEvent : EventFormattingTestBase
    {
        public override void Given()
        {
            Event = new EpcisEvent
            {
                Type = EventType.Quantity,
                BusinessLocation = "test:location",
                BusinessStep = "test:step",
                CaptureTime = new System.DateTime(2020,05,15, 12,32,32),
                EventTime = new System.DateTime(2020, 05, 15, 13, 00, 00),
                EventTimeZoneOffset = new TimeZoneOffset { Value = 60 }
            };

            Event.Epcs.Add(new Epc { Type = EpcType.Quantity, Id = "test:epc", Quantity = 5 });
            Event.SourceDestinationList.Add(new SourceDestination { Id = "test:source", Type = "source:type", Direction = SourceDestinationType.Source });
        }

        [TestMethod]
        public void ItShouldReturnAQuantityEventObject()
        {
            Assert.AreEqual("QuantityEvent", Result.Name.LocalName);
        }

        [TestMethod]
        public void TheQuantityEventShouldHaveABusinessLocationField()
        {
            Assert.IsNotNull(Result.Element("bizLocation"));
            Assert.AreEqual("test:location", Result.Element("bizLocation").Element("id").Value);
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
