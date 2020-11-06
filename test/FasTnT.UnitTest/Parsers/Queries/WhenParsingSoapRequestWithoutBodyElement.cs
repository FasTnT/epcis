using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingSoapRequestWithoutBodyElement : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("<?xml version=\"1.0\" encoding=\"utf-8\"?><soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:epcglobal:epcis-query:xsd:1\"></soapenv:Envelope>");
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }

        [TestMethod]
        public void TheExceptionShouldHaveAMeaningfulMessage()
        {
            Assert.AreEqual("Malformed SOAP request", Catched.Message);
        }
    }
}
