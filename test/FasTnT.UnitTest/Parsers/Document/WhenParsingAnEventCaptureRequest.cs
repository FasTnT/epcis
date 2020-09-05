using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Document
{
    [TestClass]
    public class WhenParsingAnEventCaptureRequest : XmlEpcisDocumentParserTestBase
    {
        public override void Given()
        {
			Request = XElement.Parse(@"
<epcis:EPCISDocument xmlns:epcis=""urn:epcglobal:epcis:xsd:1"" schemaVersion=""1.2"" creationDate=""2018-06-12T06:31:32Z"">
	<EPCISBody>
		<EventList>
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
				</bizTransactionList>
			</ObjectEvent>
		</EventList>
	</EPCISBody>
</epcis:EPCISDocument>");
        }

		[TestMethod]
		public void ItShouldReturnAnEpcisRequestObject()
        {
			Assert.IsNotNull(Result);
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldHaveTheCorrectAttributeValues()
		{
			Assert.AreEqual("1.2", Result.SchemaVersion);
			Assert.AreEqual(new System.DateTime(2018, 06, 12, 06, 31, 32), Result.DocumentTime.ToUniversalTime());
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldContainOneEvent()
		{
			Assert.AreEqual(1, Result.EventList.Count);
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldNotContainMasterdata()
		{
			Assert.AreEqual(0, Result.MasterdataList.Count);
		}
	}
}
