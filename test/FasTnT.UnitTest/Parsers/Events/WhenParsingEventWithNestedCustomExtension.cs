using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingEventWithNestedCustomExtension : XmlEventParserTestBase
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
		<testField xmlns=""https://fastnt.io/epcis"">
			<innerValue hasAttribute=""true"">7.5</innerValue>
		</testField>
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
			Assert.IsNotNull(epcisEvent.CustomFields);
			Assert.AreEqual(1, epcisEvent.CustomFields.Count);
		}

		[TestMethod]
		public void TheCustomFieldShouldOnlyHaveOneChildren()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(1, epcisEvent.CustomFields.First().Children.Count);
			Assert.AreEqual("https://fastnt.io/epcis", epcisEvent.CustomFields.First().Children.First().Namespace);
			Assert.AreEqual("innerValue", epcisEvent.CustomFields.First().Children.First().Name);
		}

		[TestMethod]
		public void TheCustomFieldChildrenShouldHaveOneAttribute()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual(1, epcisEvent.CustomFields.First().Children.First().Children.Count);
			Assert.AreEqual(FieldType.Attribute, epcisEvent.CustomFields.First().Children.First().Children.First().Type);
			Assert.AreEqual("hasAttribute", epcisEvent.CustomFields.First().Children.First().Children.First().Name);
			Assert.AreEqual("true", epcisEvent.CustomFields.First().Children.First().Children.First().TextValue);
		}

		[TestMethod]
		public void TheCustomFieldChildrenShouldHaveTheNumericValueFilledIn()
		{
			var epcisEvent = Events.First();
			Assert.AreEqual("7.5", epcisEvent.CustomFields.First().Children.First().TextValue);
			Assert.AreEqual(7.5, epcisEvent.CustomFields.First().Children.First().NumericValue);
		}
    }
}
