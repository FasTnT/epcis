using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.XmlFormatter
{
    [TestClass]
    public class WhenFormattingObjectEvent : BaseUnitTest
    {
        public XElement Result { get; set; }
        public EpcisEvent ObjectEvent { get; set; }

        public override void Arrange()
        {
            ObjectEvent = new EpcisEvent
            {
                Type = EventType.Object,
                CaptureTime = new DateTime(2018, 12, 04, 10, 17, 00),
                EventTime = new DateTime(2018, 12, 04, 10, 10, 00),
                EventTimeZoneOffset = new TimeZoneOffset { Representation = "+01:00" },
                Epcs = new List<Epc> { new Epc { Type = EpcType.List, Id = "epc1" }, new Epc { Type = EpcType.List, Id = "epc2" }},
                Action = EventAction.Add,
            };
        }

        public override void Act()
        {
            Result = new XmlEventFormatter().Format(ObjectEvent);
        }

        [Assert]
        public void TheRootElementShouldBeObjectEvent()
        {
            Assert.AreEqual("ObjectEvent", Result.Name.LocalName);
        }

        [Assert]
        public void TheXMLShouldContainAnEventTimeField()
        {
            Assert.IsNotNull(Result.Element("eventTime"));
            Assert.AreEqual("2018-12-04T10:10:00.000Z", Result.Element("eventTime").Value);
        }

        [Assert]
        public void TheXMLShouldContainARecordTimeField()
        {
            Assert.IsNotNull(Result.Element("recordTime"));
            Assert.AreEqual("2018-12-04T10:17:00.000Z", Result.Element("recordTime").Value);
        }

        [Assert]
        public void TheXMLShouldContainAnEventTimeZoneOffsetField()
        {
            Assert.IsNotNull(Result.Element("eventTimeZoneOffset"));
            Assert.AreEqual("+01:00", Result.Element("eventTimeZoneOffset").Value);
        }

        [Assert]
        public void TheXMLShouldContainAnEpcListElement()
        {
            Assert.IsNotNull(Result.Element("epcList"));
            Assert.AreEqual(2, Result.Element("epcList").Elements("epc").Count());
        }

        [Assert]
        public void TheXMLShouldContainAnActionElement()
        {
            Assert.IsNotNull(Result.Element("action"));
            Assert.AreEqual("ADD", Result.Element("action").Value);
        }
    }
}
