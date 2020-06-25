using FasTnT.Model.Events;
using FasTnT.Parsers.Xml.Formatters;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.UnitTest.Formatters.Events
{
    public abstract class EventFormattingTestBase : TestBase
    {
        public EpcisEvent Event { get; set; }
        public XElement Result { get; set; }

        public override void When()
        {
            Result = XmlEventFormatter.FormatList(new[] { Event }).FirstOrDefault();
        }
    }
}
