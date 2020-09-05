using FasTnT.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Document
{
    [TestClass]
	public class WhenParsingASubscriptionCallbackQueryTooLargeException : XmlEpcisDocumentParserTestBase
	{
		public override void Given()
		{
			Request = XElement.Parse(@"
<epcisq:EPCISQueryDocument xmlns:epcisq=""urn:epcglobal:epcis-query:xsd:1"" creationDate=""2019-01-26T20:10:01Z"" schemaVersion=""1"">
    <EPCISBody>
        <epcisq:QueryTooLargeException>
        	<reason>Too Many Results</reason>
            <queryName>SimpleEventQuery</queryName>
            <subscriptionID>SubscriptionID</subscriptionID>
        </epcisq:QueryTooLargeException>
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
		public void TheEpcisRequestObjectShouldNotContainEvents()
		{
			Assert.AreEqual(0, Result.EventList.Count);
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
			Assert.AreEqual("Too Many Results", Result.SubscriptionCallback.Reason);
			Assert.AreEqual(QueryCallbackType.QueryTooLargeException, Result.SubscriptionCallback.CallbackType);
		}
	}
}
