using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    public class XmlMasterDataFormatter
    {
        public static IEnumerable<XElement> Format(IEnumerable<EpcisMasterData> data) => data.GroupBy(md => md.Type).Select(g => new XElement("Vocabulary", new XAttribute("type", g.Key), new XElement("VocabularyElementList", g.Select(Format))));
        public static XElement Format(EpcisMasterData masterData) => new XElement("VocabularyElement", new XAttribute("id", masterData.Id), masterData.Attributes.Select(Format));
        public static XElement Format(MasterDataAttribute attribute) => new XElement("attribute", new XAttribute("id", attribute.Id), attribute.Value);
    }
}
