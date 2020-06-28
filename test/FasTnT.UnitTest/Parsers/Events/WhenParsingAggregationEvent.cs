using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
	[TestClass]
    public class WhenParsingAggregationEvent : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
    <AggregationEvent>
		<eventTime>2018-06-12T06:31:32Z</eventTime>
		<eventTimeZoneOffset>-04:00</eventTimeZoneOffset>
		<action>ADD</action>
		<parentID>urn:epc:id:sscc:005434.40000000021</parentID>
		<childEPCs>
			<epc>urn:epc:id:sgtin:005434.5121000.02</epc>
		</childEPCs>
		<bizTransactionList>
			<bizTransaction type=""urn:epcglobal:cbv:btt:desadv"">urn:epcglobal:cbv:bt:8779891013658:H9022413</bizTransaction>
			<bizTransaction type=""urn:epcglobal:cbv:btt:po"">urn:epcglobal:cbv:bt:8811891013778:PO654321</bizTransaction>
		</bizTransactionList>
		<bizStep>urn:epcglobal:cbv:bizstep:receiving</bizStep>
		<disposition>urn:epcglobal:cbv:disp:ready</disposition>
		<readPoint>
			<id>urn:epc:id:sgln:9997777.01994.1</id>
		</readPoint>
		<extension>
			<childQuantityList>
				<quantityElement>
					<epcClass>urn:epc:id:lgtin:005434.5121011</epcClass>
					<quantity>6</quantity>
					<uom>KGM</uom>
				</quantityElement>
			</childQuantityList>
			<sourceList>
				<source type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:088202.867701.0</source>
			</sourceList>
			<destinationList>
				<destination type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:8887777.01384.0</destination>
			</destinationList>
		</extension>
		<customField xmlns=""https://fastnt.io/epcis"">value</customField>
	</AggregationEvent>
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
			Assert.AreEqual(EventType.Aggregation, epcisEvent.Type);
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
		public void TheEpcsShouldBeParsedSuccessfully()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(3, epcisEvent.Epcs.Count);

			Assert.IsTrue(epcisEvent.Epcs.Any(x => x.Type == EpcType.ParentId && x.Id == "urn:epc:id:sscc:005434.40000000021"), "parentID field was not parsed correctly");
			Assert.IsTrue(epcisEvent.Epcs.Any(x => x.Type == EpcType.ChildEpc && x.Id == "urn:epc:id:sgtin:005434.5121000.02"), "childEPCs field was not parsed correctly");
			Assert.IsTrue(epcisEvent.Epcs.Any(x => x.Type == EpcType.ChildQuantity && x.Id == "urn:epc:id:lgtin:005434.5121011" && x.IsQuantity && x.Quantity == 6f && x.UnitOfMeasure == "KGM"), "quantityList field was not parsed correctly");
		}

		[TestMethod]
		public void TheActionShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(EventAction.Add, epcisEvent.Action);
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

			Assert.AreEqual(2, epcisEvent.BusinessTransactions.Count);
			Assert.IsTrue(epcisEvent.BusinessTransactions.Any(x => x.Id == "urn:epcglobal:cbv:bt:8779891013658:H9022413" && x.Type == "urn:epcglobal:cbv:btt:desadv"), "Missing bizTransaction: urn:epcglobal:cbv:btt:desadv, urn:epcglobal:cbv:bt:8779891013658:H9022413");
			Assert.IsTrue(epcisEvent.BusinessTransactions.Any(x => x.Id == "urn:epcglobal:cbv:bt:8811891013778:PO654321" && x.Type == "urn:epcglobal:cbv:btt:po"), "Missing bizTransaction: urn:epcglobal:cbv:btt:po, urn:epcglobal:cbv:bt:8811891013778:PO654321");
		}

		[TestMethod]
		public void TheSourceDestListShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();

			Assert.AreEqual(2, epcisEvent.SourceDestinationList.Count);
			Assert.IsTrue(epcisEvent.SourceDestinationList.Any(x => x.Direction == SourceDestinationType.Source && x.Type == "urn:epcglobal:cbv:sdt:owning_party" && x.Id == "urn:epc:id:sgln:088202.867701.0"), "Missing Source: urn:epcglobal:cbv:sdt:owning_party, urn:epc:id:sgln:088202.867701.0");
			Assert.IsTrue(epcisEvent.SourceDestinationList.Any(x => x.Direction == SourceDestinationType.Destination && x.Type == "urn:epcglobal:cbv:sdt:owning_party" && x.Id == "urn:epc:id:sgln:8887777.01384.0"), "Missing Destination: urn:epcglobal:cbv:sdt:owning_party, urn:epc:id:sgln:8887777.01384.0");
		}

		[TestMethod]
		public void TheCustomFieldsShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();

			Assert.AreEqual(1, epcisEvent.CustomFields.Count);
			Assert.IsTrue(epcisEvent.CustomFields.Any(x => x.Type == FieldType.CustomField && x.Namespace == "https://fastnt.io/epcis" && x.Name == "customField"), "Missing customField: https://fastnt.io/epcis#customField");
			Assert.AreEqual("value", epcisEvent.CustomFields.First().TextValue);
		}
    }}
