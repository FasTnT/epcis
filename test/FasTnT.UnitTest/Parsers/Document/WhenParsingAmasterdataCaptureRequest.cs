using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Document
{
    [TestClass]
    public class WhenParsingAmasterdataCaptureRequest : XmlEpcisDocumentParserTestBase
    {
        public override void Given()
        {
			Request = XElement.Parse(@"<epcismd:EPCISMasterDataDocument xmlns:epcismd=""urn:epcglobal:epcis-masterdata:xsd:1"" schemaVersion=""1.0"" creationDate=""2005-07-11T11:30:47.0Z"">
 <EPCISBody> 
   <VocabularyList> 
     <Vocabulary type=""urn:epcglobal:epcis:vtype:BusinessLocation""> 
       <VocabularyElementList> 
         <VocabularyElement id=""urn:epc:id:sgln:0037000.00729.0""> 
          <attribute id=""http://epcis.example.com/mda/latitude"">+18.0000</attribute> 
          <attribute id=""http://epcis.example.com/mda/longitude"">-70.0000</attribute>
         </VocabularyElement>
       </VocabularyElementList>
     </Vocabulary>
   </VocabularyList>
 </EPCISBody>
</epcismd:EPCISMasterDataDocument>");
        }

		[TestMethod]
		public void ItShouldReturnAnEpcisRequestObject()
        {
			Assert.IsNotNull(Result);
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldHaveTheCorrectAttributeValues()
		{
			Assert.AreEqual("1.0", Result.SchemaVersion);
			Assert.AreEqual(new System.DateTime(2005, 07, 11, 11, 30, 47), Result.DocumentTime.ToUniversalTime());
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldContainOneMasterdata()
		{
			Assert.AreEqual(1, Result.MasterdataList.Count);
		}

		[TestMethod]
		public void TheEpcisRequestObjectShouldNotContainEvents()
		{
			Assert.AreEqual(0, Result.EventList.Count);
		}
	}
}
