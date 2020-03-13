using FasTnT.Commands.Requests;
using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Parsers.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FasTnT.UnitTest.Parsers
{
    [TestClass]
    public class WhenParsingXmlPollRequest : TestBase
    {
        public MemoryStream PollStream { get; set; }
        public IQueryRequest Result { get; set; }
        public override void Given()
        {
            PollStream = new MemoryStream();
            var sw = new StreamWriter(PollStream);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?><epcisq:EPCISQueryDocument xmlns:epcisq=\"urn:epcglobal:epcis-query:xsd:1\" creationDate=\"2019-01-26T20:10:01.8111457Z\" schemaVersion=\"1\"><EPCISBody><epcisq:Poll><queryName>SimpleEventQuery</queryName><params><param><name>EQ_bizStep</name><value><value>urn:epcglobal:cbv:bizstep:packing</value></value></param></params></epcisq:Poll></EPCISBody></epcisq:EPCISQueryDocument>");
            sw.Flush();
            PollStream.Seek(0, SeekOrigin.Begin);
        }

        public override void When()
        {
            Result = new XmlQueryParser().Read(PollStream, default).Result;
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

            Assert.AreEqual(1, request.Parameters.Length);
        }
    }
}
