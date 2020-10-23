using FasTnT.Model.MasterDatas;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public static class XmlMasterdataFormatter
    {
        internal static IEnumerable<XElement> FormatMasterData(IList<EpcisMasterData> masterdataList, CancellationToken cancellationToken)
        {
            var vocabularies = masterdataList.GroupBy(md => md.Type);

            return vocabularies.TakeWhile(_ => !cancellationToken.IsCancellationRequested).Select(FormatGroup);
        }

        private static XElement FormatGroup(IGrouping<string, EpcisMasterData> group)
        {
            var elements = new XElement("VocabularyElementList", group.Select(FormatVocabulary));

            return new XElement("Vocabulary", new XAttribute("type", group.Key), elements);
        }

        private static XElement FormatVocabulary(EpcisMasterData masterData)
        {
            var attributes = masterData.Attributes.Select(Format);
            var children = FormatChildren(masterData.Children);

            return new XElement("VocabularyElement", new XAttribute("id", masterData.Id), attributes, children);
        }

        private static XElement FormatChildren(List<string> children)
        {
            return children.Any() ? new XElement("children", children.Select(x => new XElement("id", x))) : null;
        }

        private static XElement Format(MasterDataAttribute attribute)
        {
            return new XElement("attribute", new XAttribute("id", attribute.Id), attribute.Value);
        }
    }
}
