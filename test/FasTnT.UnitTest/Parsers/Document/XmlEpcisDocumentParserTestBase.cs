using FasTnT.Model;
using FasTnT.Parsers.Xml.Capture;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Document
{
    public abstract class XmlEpcisDocumentParserTestBase : TestBase
    {
        public EpcisRequest Result { get; set; }
        public XElement Request { get; set; }

        public override void When()
        {
            Result = XmlEpcisDocumentParser.Parse(Request);
        }
    }
}
