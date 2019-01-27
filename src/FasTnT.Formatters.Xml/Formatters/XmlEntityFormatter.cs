using FasTnT.Model;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    public class XmlEntityFormatter
    {
        public static IEnumerable<XElement> Format(IEnumerable<IEntity> entity) => entity.Any() ? Format((dynamic)entity) : null;
        public static IEnumerable<XElement> Format(IEnumerable<EpcisEvent> events) => events.Select(XmlEventFormatter.Format);
        public static IEnumerable<XElement> Format(IEnumerable<EpcisMasterData> masterData) => XmlMasterDataFormatter.Format(masterData);
    }
}
