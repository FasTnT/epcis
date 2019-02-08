using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Formatters.Xml.Requests
{
    public class XmlMasterDataParser
    {
        private int _internalId = 0;

        public IList<EpcisMasterData> ParseMasterDatas(IEnumerable<XElement> elements)
        {
            var parsedElements = new List<EpcisMasterData>();

            foreach(var element in elements)
            {
                parsedElements.AddRange(ParseVocabularyElements(element.Attribute("type").Value, element.Element("VocabularyElementList").Elements("VocabularyElement")));
            }

            return parsedElements;
        }

        private IEnumerable<EpcisMasterDataHierarchy> ParseHierarchy(IEnumerable<XElement> elements)
        {
            return elements?.Select(x => new EpcisMasterDataHierarchy { ChildrenId = x.Value }) ?? new EpcisMasterDataHierarchy[0];
        }

        private IEnumerable<EpcisMasterData> ParseVocabularyElements(string type, IEnumerable<XElement> elements)
        {
            return elements.Select(e =>
            {
                _internalId = 0;
                var masterData = new EpcisMasterData { Id = e.Attribute("id").Value, Type = type };
                masterData.Attributes = ParseAttributes(e.Elements("attribute"), masterData).ToList();
                masterData.Children = ParseHierarchy(e.Element("children")?.Elements("id")).ToList();
                return masterData;
            });
        }

        private IEnumerable<MasterDataAttribute> ParseAttributes(IEnumerable<XElement> elements, EpcisMasterData masterData)
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

        private IEnumerable<MasterDataField> ParseField(IEnumerable<XElement> elements, MasterDataAttribute attribute, int? parentId = null)
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
