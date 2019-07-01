using System.Linq;
using System.Xml.Linq;
using FasTnT.Formatters.Xml;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.FormattersTests.SoapFormatterTests
{
    [TestClass]
    public class WhenFormattingAGetQueryNamesResponse : BaseUnitTest
    {
        public IEpcisResponse Response { get; set; }
        public SoapResponseFormatter Formatter { get; set; }
        public XElement Formatted { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Formatter = new SoapResponseFormatter();
            Response = new GetQueryNamesResponse { QueryNames = new[] { "SimpleEventQuery", "SimpleMasterdataQuery" } };
        }

        public override void Act()
        {
            Formatted = Formatter.Format(Response);
        }

        [Assert]
        public void TheSoapResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Formatted);
        }

        [Assert]
        public void TheResponseRootShouldBeAGetQueryNamesResult()
        {
            Assert.AreEqual(XName.Get("GetQueryNamesResult", "urn:epcglobal:epcis-query:xsd:1"), Formatted.Name);
        }

        [Assert]
        public void TheResponseShouldContainAllTheQueryNames()
        {
            Assert.AreEqual(2, Formatted.Elements().Count());
        }

        [Assert]
        public void TheResponseShouldContainTheCorrectQueryNames()
        {
            Assert.IsNotNull(Formatted.Elements().Where(x => x.Value == "SimpleEventQuery"));
            Assert.IsNotNull(Formatted.Elements().Where(x => x.Value == "SimpleMasterdataQuery"));
        }
    }
}
