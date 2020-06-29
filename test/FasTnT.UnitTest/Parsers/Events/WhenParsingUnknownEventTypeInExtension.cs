using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    [TestClass]
    public class WhenParsingUnknownEventTypeInExtension : XmlEventParserTestBase
    {
        public override void Given()
        {
            XmlEventList = XElement.Parse(@"<EventList>
<extension>
    <UnknownEvent>
		<eventTime>2018-06-12T06:31:32Z</eventTime>
		<eventTimeZoneOffset>-04:00</eventTimeZoneOffset>
		<epcList>
			<epc>urn:epc:id:sscc:005434.40000000021</epc>
		</epcList>
		<action>OBSERVE</action>
		<testField xmlns=""https://fastnt.io/epcis"">
			<innerValue hasAttribute=""true"">7.5</innerValue>
		</testField>
	</UnknownEvent>
</extension>
</EventList>");
        }

		[TestMethod]
		public void ItShouldNotReturnAnyEvent()
		{
			Assert.AreEqual(0, Events.Count());
		}

		[TestMethod]
		public void ItShouldThrowAnException()
		{
			Assert.IsNotNull(Catched);
			Assert.AreEqual("Element 'UnknownEvent' not expected in this context", Catched.Message);
		}
    }
}
