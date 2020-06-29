using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingEventWithBaseExtension : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
    <ObjectEvent>
		<eventTime>2018-06-12T06:31:32Z</eventTime>
		<eventTimeZoneOffset>-04:00</eventTimeZoneOffset>
		<baseExtension>
			<eventID>TestEventID</eventID>
			<errorDeclaration>
				<declarationTime>2020-06-28T10:06:02Z</declarationTime>
				<reason>Test reason corrective event</reason>
				<correctiveEventIDs>
					<correctiveEventID>Event1</correctiveEventID>
					<correctiveEventID>Event2</correctiveEventID>
				</correctiveEventIDs>
			</errorDeclaration>
		</baseExtension>
		<epcList>
			<epc>urn:epc:id:sscc:005434.40000000021</epc>
		</epcList>
		<action>OBSERVE</action>
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
		public void TheEventShouldHaveTheCorrectiveReasonParsed()
		{
			var epcisEvent = Events.First();
			Assert.IsNotNull(epcisEvent.CorrectiveReason);
			Assert.AreEqual("Test reason corrective event", epcisEvent.CorrectiveReason);
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectiveDeclarationTimeParsed()
		{
			var epcisEvent = Events.First();
			Assert.IsNotNull(epcisEvent.CorrectiveDeclarationTime);
			Assert.AreEqual(new DateTime(2020, 06, 28, 10, 06, 02, DateTimeKind.Utc), epcisEvent.CorrectiveDeclarationTime.Value.ToUniversalTime());
		}

		[TestMethod]
		public void TheEventShouldHaveTheCorrectiveEventIdsParsed()
		{
			var epcisEvent = Events.First();
			Assert.IsNotNull(epcisEvent.CorrectiveEventIds);
			Assert.AreEqual(2, epcisEvent.CorrectiveEventIds.Count);
			Assert.IsTrue(epcisEvent.CorrectiveEventIds.Any(x => x == "Event1"), "Event1 should be parsed as CorrectiveEventID");
			Assert.IsTrue(epcisEvent.CorrectiveEventIds.Any(x => x == "Event2"), "Event2 should be parsed as CorrectiveEventID");
		}
    }
}
