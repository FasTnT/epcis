using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Parsers.Xml.Capture
{
    public static class XmlMasterDataParser
    {
        public static IList<EpcisMasterData> ParseMasterDatas(IEnumerable<XElement> elements)
        {
            var parsedElements = new List<EpcisMasterData>();

            foreach(var element in elements)
            {
                parsedElements.AddRange(ParseVocabularyElements(element.Attribute("type").Value, element.Element("VocabularyElementList").Elements("VocabularyElement")));
            }

            return parsedElements;
        }

        private static IEnumerable<EpcisMasterDataHierarchy> ParseHierarchy(IEnumerable<XElement> elements)
        {
            return elements?.Select(x => new EpcisMasterDataHierarchy { ChildrenId = x.Value }) ?? new EpcisMasterDataHierarchy[0];
        }

        private static IEnumerable<EpcisMasterData> ParseVocabularyElements(string type, IEnumerable<XElement> elements)
        {
            return elements.Select(e =>
            {
                var masterData = new EpcisMasterData { Id = e.Attribute("id").Value, Type = type };
                masterData.Attributes = ParseAttributes(e.Elements("attribute"), masterData).ToList();
                masterData.Children = ParseHierarchy(e.Element("children")?.Elements("id")).ToList();
                return masterData;
            });
        }

        private static IEnumerable<MasterDataAttribute> ParseAttributes(IEnumerable<XElement> elements, EpcisMasterData masterData)
        {
            return elements.Select(element =>
            {
                var attr = new MasterDataAttribute
                {
                    ParentId = masterData.Id,
                    ParentType = masterData.Type,
                    Id = element.Attribute("id").Value,
                    Value = element.Value
                };

                ParseField(element.Elements(), attr.Fields, attr);

                return attr;
            });
        }

        private static void ParseField(IEnumerable<XElement> elements, IList<MasterDataField> output, MasterDataAttribute attribute)
        {
            if (elements == null || !elements.Any()) return;

            foreach(var element in elements)
            {
                var field = new MasterDataField
                {
                    Name = element.Name.LocalName,
                    Namespace = element.Name.NamespaceName,
                    ParentId = attribute.Id,
                    MasterdataId = attribute.ParentId,
                    MasterdataType = attribute.ParentType,
                    Value = element.HasElements ? null : element.Value
                };

                output.Add(field);
                ParseField(element.Elements(), field.Children, attribute);
            }
        }
    }
}
