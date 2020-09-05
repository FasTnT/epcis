using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingQuantityEvent : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
    <QuantityEvent>
		<eventTime>2018-06-12T07:31:32Z</eventTime>
		<eventTimeZoneOffset>-05:00</eventTimeZoneOffset>
		<bizStep>urn:epcglobal:cbv:bizstep:shipping</bizStep>
		<disposition>urn:epcglobal:cbv:disp:in_transit</disposition>
		<readPoint>
			<id>urn:epc:id:sgln:9997777.01994.0</id>
		</readPoint>
		<epcClass>urn:epc:id:gtin:005434.40000000021</epcClass>
		<quantity>4.454</quantity>
		<testField xmlns=""https://fastnt.io/epcis"">QuantityEvent custom field</testField>
	</QuantityEvent>
</EventList>");
        }

		[TestMethod]
		public void ItShouldReturnOneEvent()
		{
			Assert.AreEqual(1, Events.Count());
		}

		[TestMethod]
		public void TheEventShouldBeOfTypeQuantityEvent()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(EventType.Quantity, epcisEvent.Type);
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectDate()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(new DateTime(2018, 06, 12,07, 31,32, DateTimeKind.Utc), epcisEvent.EventTime.ToUniversalTime());
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectTimezoneOffset()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(-300, epcisEvent.EventTimeZoneOffset.Value);
		}

		[TestMethod]
		public void TheEpcListShouldBeParsedSuccessfully()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(1, epcisEvent.Epcs.Count);
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
		public void TheCustomFieldsShouldBeParsedCorrectly()
		{
			var epcisEvent = Events.First();

			Assert.AreEqual(1, epcisEvent.CustomFields.Count);
			Assert.IsTrue(epcisEvent.CustomFields.Any(x => x.Type == FieldType.CustomField && x.Namespace == "https://fastnt.io/epcis" && x.Name == "testField"), "Missing customField: https://fastnt.io/epcis#testField");
			Assert.AreEqual("QuantityEvent custom field", epcisEvent.CustomFields.First().TextValue);
		}
    }
}
