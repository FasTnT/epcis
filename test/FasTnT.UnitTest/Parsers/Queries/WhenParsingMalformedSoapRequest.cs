using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Parsers.Soap
{
    [TestClass]
    public class WhenParsingMalformedSoapRequest : SoapParserTestBase
    {
        public override void Given()
        {
            SetRequest("{}");
        }

        [TestMethod]
        public void ItShouldThrowAnException()
        {
            Assert.IsNotNull(Catched);
        }
    }
}
