using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingTransformationEvent : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
<extension>
    <TransformationEvent>
		<eventTime>2018-06-12T06:31:32Z</eventTime>
		<eventTimeZoneOffset>-04:00</eventTimeZoneOffset>
		<transformationID>transformationID</transformationID>
		<bizTransactionList>
			<bizTransaction type=""urn:epcglobal:cbv:btt:desadv"">urn:epcglobal:cbv:bt:8779891013658:H9022413</bizTransaction>
		</bizTransactionList>
		<sourceList>
			<source type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:088202.867701.0</source>
		</sourceList>
		<destinationList>
			<destination type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:8887777.01384.0</destination>
		</destinationList>
		<inputEPCList>
			<epc>urn:epc:id:sgtin:005434.5121000.02</epc>
		</inputEPCList>
		<outputEPCList>
			<epc>urn:epc:id:sgtin:005434.5121000.12</epc>
		</outputEPCList>
		<bizStep>urn:epcglobal:cbv:bizstep:receiving</bizStep>
		<disposition>urn:epcglobal:cbv:disp:ready</disposition>
		<readPoint>
			<id>urn:epc:id:sgln:9997777.01994.1</id>
		</readPoint>
	</TransformationEvent>
</extension>
</EventList>");
        }

		[TestMethod]
		public void ItShouldReturnOneEvent()
		{
			Assert.AreEqual(1, Events.Count());
		}

		[TestMethod]
		public void TheEventShouldBeCorrect()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(EventType.Transformation, epcisEvent.Type);
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectDate()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(new DateTime(2018, 06, 12,06, 31,32, DateTimeKind.Utc), epcisEvent.EventTime.ToUniversalTime());
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectTimezoneOffset()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(-240, epcisEvent.EventTimeZoneOffset.Value);
		}

		[TestMethod]
		public void TheTransformationIDShouldBeParsed()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("transformationID", epcisEvent.TransformationId);
		}

		[TestMethod]
		public void TheEpcsShouldBeParsedSuccessfully()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(2, epcisEvent.Epcs.Count);

			Assert.IsTrue(epcisEvent.Epcs.Any(x => x.Type == EpcType.InputEpc && x.Id == "urn:epc:id:sgtin:005434.5121000.02"), "inputEPCList field was not parsed correctly");
			Assert.IsTrue(epcisEvent.Epcs.Any(x => x.Type == EpcType.OutputEpc && x.Id == "urn:epc:id:sgtin:005434.5121000.12"), "outputEPCList field was not parsed correctly");
		}

		[TestMethod]
		public void TheActionShouldBeNull()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(null, epcisEvent.Action);
		}

		[TestMethod]
		public void TheBizStepShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epcglobal:cbv:bizstep:receiving", epcisEvent.BusinessStep);
		}

		[TestMethod]
		public void TheDispositionShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epcglobal:cbv:disp:ready", epcisEvent.Disposition);
		}

		[TestMethod]
		public void TheReadPointShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epc:id:sgln:9997777.01994.1", epcisEvent.ReadPoint);
		}

		[TestMethod]
		public void TheTransactionsShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();

			Assert.AreEqual(1, epcisEvent.BusinessTransactions.Count);
			Assert.IsTrue(epcisEvent.BusinessTransactions.Any(x => x.Id == "urn:epcglobal:cbv:bt:8779891013658:H9022413" && x.Type == "urn:epcglobal:cbv:btt:desadv"), "Missing bizTransaction: urn:epcglobal:cbv:btt:desadv, urn:epcglobal:cbv:bt:8779891013658:H9022413");
		}

		[TestMethod]
		public void TheSourceDestListShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();

			Assert.AreEqual(2, epcisEvent.SourceDestinationList.Count);
			Assert.IsTrue(epcisEvent.SourceDestinationList.Any(x => x.Direction == SourceDestinationType.Source && x.Type == "urn:epcglobal:cbv:sdt:owning_party" && x.Id == "urn:epc:id:sgln:088202.867701.0"), "Missing Source: urn:epcglobal:cbv:sdt:owning_party, urn:epc:id:sgln:088202.867701.0");
			Assert.IsTrue(epcisEvent.SourceDestinationList.Any(x => x.Direction == SourceDestinationType.Destination && x.Type == "urn:epcglobal:cbv:sdt:owning_party" && x.Id == "urn:epc:id:sgln:8887777.01384.0"), "Missing Destination: urn:epcglobal:cbv:sdt:owning_party, urn:epc:id:sgln:8887777.01384.0");
		}
    }
}
