using FasTnT.Formatters.Xml.Parsers.Capture.Events;
using FasTnT.Model.Events;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
	public abstract class XmlEventParserTestBase : TestBase
    {
		public IEnumerable<EpcisEvent> Events { get; set; }
		public XElement XmlEventList { get; set; }

        public override void When()
        {
            Events = XmlEventParser.ParseEvents(XmlEventList);
        }
    }
}
