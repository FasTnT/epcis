using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.UnitTest.Formatters.Events
{
    [TestClass]
    public class WhenFormattingATransactionEvent : EventFormattingTestBase
    {
        public override void Given()
        {
            Event = new EpcisEvent
            {
                Type = EventType.Transaction,
                Action = EventAction.Observe,
                BusinessLocation = "test:trans_location",
                BusinessStep = "test:step",
                BusinessTransactions = new List<BusinessTransaction> { new BusinessTransaction { Id = "test_trans", Type = "trans_type" } },
                CaptureTime = new System.DateTime(2020, 05, 15, 12, 32, 32),
                EventTime = new System.DateTime(2020, 07, 15, 13, 00, 00),
                EventTimeZoneOffset = new TimeZoneOffset { Value = -60 }
            };

            Event.SourceDestinationList.Add(new SourceDestination { Id = "test:source", Type = "source:type", Direction = SourceDestinationType.Source });
        }

        [TestMethod]
        public void ItShouldReturnATransactionEventObject()
        {
            Assert.AreEqual("TransactionEvent", Result.Name.LocalName);
        }

        [TestMethod]
        public void TheAggregationEventShouldHaveABizTransactionListField()
        {
            Assert.IsNotNull(Result.Element("bizTransactionList"));
            Assert.AreEqual(1, Result.Element("bizTransactionList").Elements("bizTransaction").Count());
            Assert.AreEqual("trans_type", Result.Element("bizTransactionList").Elements("bizTransaction").First().Attribute("type").Value);
            Assert.AreEqual("test_trans", Result.Element("bizTransactionList").Elements("bizTransaction").First().Value);
        }

        [TestMethod]
        public void TheAggregationEventShouldHaveABusinessLocationField()
        {
            Assert.IsNotNull(Result.Element("bizLocation"));
            Assert.AreEqual("test:trans_location", Result.Element("bizLocation").Element("id").Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTime()
        {
            Assert.IsNotNull(Result.Element("eventTime"));
            Assert.AreEqual("2020-07-15T13:00:00Z", Result.Element("eventTime").Value);
        }

        [TestMethod]
        public void TheEventShouldHaveTheCorrectEventTimeZoneOffset()
        {
            Assert.IsNotNull(Result.Element("eventTimeZoneOffset"));
            Assert.AreEqual("-01:00", Result.Element("eventTimeZoneOffset").Value);
        }
    }
}
