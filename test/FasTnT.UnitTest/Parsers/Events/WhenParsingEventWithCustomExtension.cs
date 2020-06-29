using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingEventWithCustomExtension : XmlEventParserTestBase
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
		<extension>
			<extension>
				<test>value</test>
			</extension>
		</extension>
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
		public void TheEventShouldHaveOneCustomField()
		{
			var epcisEvent = Events.First();
			Assert.IsNotNull(epcisEvent.CustomFields.Count);
			Assert.AreEqual(FieldType.Extension, epcisEvent.CustomFields.First().Type);
		}
    }
}
