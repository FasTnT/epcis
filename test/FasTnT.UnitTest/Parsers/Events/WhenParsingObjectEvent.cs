using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
	[TestClass]
    public class WhenParsingObjectEvent : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
    <ObjectEvent>
		<eventTime>2018-06-12T06:31:32Z</eventTime>
		<eventTimeZoneOffset>-04:00</eventTimeZoneOffset>
		<epcList>
			<epc>urn:epc:id:sscc:005434.40000000021</epc>
		</epcList>
		<action>OBSERVE</action>
		<bizStep>urn:epcglobal:cbv:bizstep:shipping</bizStep>
		<disposition>urn:epcglobal:cbv:disp:in_transit</disposition>
		<readPoint>
			<id>urn:epc:id:sgln:9997777.01994.0</id>
		</readPoint>
		<bizTransactionList>
			<bizTransaction type=""urn:epcglobal:cbv:btt:desadv"">urn:epcglobal:cbv:bt:8779891013658:H9022413</bizTransaction>
			<bizTransaction type=""urn:epcglobal:cbv:btt:po"">urn:epcglobal:cbv:bt:8811891013778:PO654321</bizTransaction>
		</bizTransactionList>
		<extension>
			<sourceList>
				<!--owning_party = from business party -->
				<source type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:088202.867701.0</source>
			</sourceList>
			<destinationList>
				<!-- owning_paty = to business party -->
				<destination type=""urn:epcglobal:cbv:sdt:owning_party"">urn:epc:id:sgln:8887777.01384.0</destination>
			</destinationList>
		</extension>
		<customField xmlns=""https://fastnt.io/epcis"">Customfield value</customField>
	</ObjectEvent>
</EventList>");
        }

		[TestMethod]
		public void ItShouldReturnOneEvent()
		{
			Assert.AreEqual(1, Events.Count());
		}

		[TestMethod]
		public void TheEventShouldBeOfTypeObjectEvent()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(EventType.Object, epcisEvent.Type);
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
		public void TheEpcListShouldBeParsedSuccessfully()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(1, epcisEvent.Epcs.Count);
		}

		[TestMethod]
		public void TheActionShouldBeObserve()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(EventAction.Observe, epcisEvent.Action);
		}

		[TestMethod]
		public void TheBizStepShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epcglobal:cbv:bizstep:shipping", epcisEvent.BusinessStep);
		}

		[TestMethod]
		public void TheDispositionShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epcglobal:cbv:disp:in_transit", epcisEvent.Disposition);
		}

		[TestMethod]
		public void TheReadPointShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("urn:epc:id:sgln:9997777.01994.0", epcisEvent.ReadPoint);
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
			Assert.AreEqual("Customfield value", epcisEvent.CustomFields.First().TextValue);
		}
    }
}
