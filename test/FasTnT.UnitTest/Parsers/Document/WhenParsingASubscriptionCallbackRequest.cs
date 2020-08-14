using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Document
{
    [TestClass]
	public class WhenParsingASubscriptionCallbackRequest : XmlEpcisDocumentParserTestBase
	{
		public override void Given()
		{
			Request = XElement.Parse(@"
<epcisq:EPCISQueryDocument xmlns:epcisq=""urn:epcglobal:epcis-query:xsd:1"" creationDate=""2019-01-26T20:10:01Z"" schemaVersion=""1"">
    <EPCISBody>
        <epcisq:QueryResults>
            <queryName>SimpleEventQuery</queryName>
            <subscriptionID>SubscriptionID</subscriptionID>
            <resultsBody>
                <EventList>
                    <extension>
                        <TransformationEvent>
                            <eventTime>2018-06-14T14:51:17.000Z</eventTime>
                            <recordTime>2019-01-25T13:35:26.157Z</recordTime>
                            <eventTimeZoneOffset>+01:00</eventTimeZoneOffset>
                            <inputEPCList>
                                <epc>urn:epc:id:sscc:4001356.5900000817</epc>
                            </inputEPCList>
                            <outputQuantityList>
                                <quantityElement>
                                    <epcClass>urn:epc:id:sscc:4001356.5900000818</epcClass>
                                    <quantity>15</quantity>
                                    <uom>KGM</uom>
                                </quantityElement>
                            </outputQuantityList>
                            <bizStep>urn:fastnt:demo:bizstep:demo:packing</bizStep>
                            <disposition>urn:epcglobal:cbv:disp:loading</disposition>
                            <readPoint>
                                <id>urn:fastnt:demo:readpoint:0037000.00729.210,432</id>
                            </readPoint>
                            <bizLocation>
                                <id>urn:epc:id:sgln:0037000.00729.0</id>
                            </bizLocation>
                            <customEvent xmlns=""http://epcis.fastnt.com/custom"">Test</customEvent>
                        </TransformationEvent>
                    </extension>
                </EventList>
            </resultsBody>
        </epcisq:QueryResults>
    </EPCISBody>
</epcisq:EPCISQueryDocument>");
		}

		[TestMethod]
		public void ItShouldReturnAnEpcisRequestObject()
		{
			Assert.IsNotNull(Result);
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldHaveTheCorrectAttributeValues()
		{
			Assert.AreEqual("1", Result.SchemaVersion);
			Assert.AreEqual(new System.DateTime(2019, 01, 26, 20, 10, 01), Result.DocumentTime.ToUniversalTime());
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

		[TestMethod]
		public void TheEpcisRequestObjectShouldHaveTheSubscriptionCallbackFilled()
		{
			Assert.IsNotNull(Result.SubscriptionCallback);
			Assert.AreEqual("SubscriptionID", Result.SubscriptionCallback.SubscriptionId);
			Assert.AreEqual(QueryCallbackType.Success, Result.SubscriptionCallback.CallbackType);
		}
	}
}
