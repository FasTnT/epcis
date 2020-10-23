using FasTnT.Formatters.Xml.Parsers.Capture.Events;
using FasTnT.Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Parsers.Events
{
    public abstract class XmlEventParserTestBase : TestBase
    {
        public IEnumerable<EpcisEvent> Events { get; set; } = new List<EpcisEvent>();
		public XElement XmlEventList { get; set; }
        public Exception Catched { get; set; }

        public override void When()
        {
            try
            {
                Events = XmlEventParser.ParseEvents(XmlEventList).ToList();
            }
            catch(Exception ex)
            {
                Catched = ex;
            }
        }
    }
}
