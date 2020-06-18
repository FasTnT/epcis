using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.UnitTest.Parsers.XML
{
    [TestClass]
    public class WhenParsingXmlPollRequest : XmlParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><epcisq:EPCISQueryDocument xmlns:epcisq=\"urn:epcglobal:epcis-query:xsd:1\" creationDate=\"2019-01-26T20:10:01.8111457Z\" schemaVersion=\"1\"><EPCISBody><epcisq:Poll><queryName>SimpleEventQuery</queryName><params><param><name>EQ_bizStep</name><value><value>urn:epcglobal:cbv:bizstep:packing</value></value></param></params></epcisq:Poll></EPCISBody></epcisq:EPCISQueryDocument>");
        }

        [TestMethod]
        public void ItShouldReturnAPollRequest()
        {
            Assert.IsInstanceOfType(Result, typeof(PollRequest));
        }

        [TestMethod]
        public void TheRequestShouldContainTheParameters()
        {
            var request = (PollRequest)Result;

            Assert.AreEqual(1, request.Parameters.Count());
        }
    }
}
