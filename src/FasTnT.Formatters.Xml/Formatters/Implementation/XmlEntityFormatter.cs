using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters.Implementation
{
    public class XmlEntityFormatter
    {
        public static IEnumerable<XElement> FormatEvents(IEnumerable<EpcisEvent> events) => events.Select(new XmlEventFormatter().Format);
        public static IEnumerable<XElement> FormatMasterData(IEnumerable<EpcisMasterData> masterData) => XmlMasterDataFormatter.Format(masterData);
    }
}
