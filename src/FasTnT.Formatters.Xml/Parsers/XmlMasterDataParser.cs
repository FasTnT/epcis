using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Formatters.Xml.Requests
{
    public static class XmlMasterDataParser
    {
        private static int _internalId = 0;

        public static IList<EpcisMasterData> ParseMasterDatas(IEnumerable<XElement> elements)
        {
            var parsedElements = new List<EpcisMasterData>();

            foreach(var element in elements)
            {
                parsedElements.AddRange(ParseVocabularyElements(element.Attribute("type").Value, element.Element("VocabularyElementList").Elements("VocabularyElement")));
            }

            return parsedElements;
        }

        public static IList<EpcisMasterDataHierarchy> ParseMasterDataHierarchy(IEnumerable<XElement> elements)
        {
            var parsedElements = new List<EpcisMasterDataHierarchy>();

            foreach (var element in elements)
            {
                parsedElements.AddRange(ParseHierarchy(element.Attribute("type").Value, element.Element("VocabularyElementList").Elements("VocabularyElement")));
            }

            return parsedElements;
        }

        private static IEnumerable<EpcisMasterDataHierarchy> ParseHierarchy(string type, IEnumerable<XElement> elements)
        {
            return elements.SelectMany(e => {
                return e.Element("children") != null ?
                e.Element("children")?.Elements("id")?.Select(x =>
                {
                    return new EpcisMasterDataHierarchy { Type = type, ParentId = e.Attribute("id").Value, ChildrenId = x.Value };
                })
                : new EpcisMasterDataHierarchy[0];
            });
        }

        private static IEnumerable<EpcisMasterData> ParseVocabularyElements(string type, IEnumerable<XElement> elements)
        {
            return elements.Select(e =>
            {
                _internalId = 0;
                var masterData = new EpcisMasterData { Id = e.Attribute("id").Value, Type = type };
                masterData.Attributes = ParseAttributes(e.Elements("attribute"), masterData).ToList();

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

                if (element.HasElements) attr.Fields.AddRange(ParseField(element.Elements(), attr));

                return attr;
            });
        }

        private static IEnumerable<MasterDataField> ParseField(IEnumerable<XElement> elements, MasterDataAttribute attribute, int? parentId = null)
        {
            var fields = new List<MasterDataField>();

            foreach(var element in elements)
            {
                fields.Add(new MasterDataField
                {
                    Id = _internalId++,
                    InternalParentId = parentId,
                    Name = element.Name.LocalName,
                    Namespace = element.Name.NamespaceName,
                    ParentId = attribute.Id,
                    MasterdataId = attribute.ParentId,
                    MasterdataType = attribute.ParentType,
                    Value = element.HasElements ? null : element.Value
                });
                fields.AddRange(ParseField(element.Elements(), attribute, fields.Last().Id));
            }

            return fields;
        }
    }
}
