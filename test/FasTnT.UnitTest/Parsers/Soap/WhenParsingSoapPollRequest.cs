using FasTnT.Commands.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingXmlPollRequest : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:epcglobal:epcis-query:xsd:1\"><soapenv:Body><urn:Poll><queryName>SimpleEventQuery</queryName><params><param><name>EQ_bizStep</name><value><value>urn:epcglobal:cbv:bizstep:packing</value></value></param></params></urn:Poll></soapenv:Body></soapenv:Envelope>");
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
