using FasTnT.Model.Events;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters.Implementation
{
    public static class XmlEntityFormatter
    {
        public static IEnumerable<XElement> FormatEvents(IEnumerable<EpcisEvent> events) => events.Select(XmlEventFormatter.Format);
        public static IEnumerable<XElement> FormatMasterData(IEnumerable<EpcisMasterData> masterData) => XmlMasterDataFormatter.Format(masterData);
    }
}
