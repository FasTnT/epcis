using FasTnT.Model.MasterDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Parsers.Capture.Events
{
    public static class XmlMasterdataParser
    {
        public static IEnumerable<EpcisMasterData> ParseMasterdata(XElement root)
        {
            var list = new List<EpcisMasterData>();

            foreach(var element in root.Elements("Vocabulary"))
            {
                list.AddRange(ParseVocabulary(element));
            }

            return list;
        }

        private static IEnumerable<EpcisMasterData> ParseVocabulary(XElement element)
        {
            var type = element.Attribute("type").Value;

            foreach(var vocElement in element.Element("VocabularyElementList")?.Elements("VocabularyElement"))
            {
                yield return ParseVocabularyElement(vocElement, type);
            }
        }

        private static EpcisMasterData ParseVocabularyElement(XElement element, string type)
        {
            var masterdata = new EpcisMasterData
            {
                Type = type,
                Id = element.Attribute("id").Value
            };

            masterdata.Attributes.AddRange(element.Elements("attribute").Select(ParseAttribute));
            masterdata.Children.AddRange(ParseChildren(element.Element("children")));

            return masterdata;
        }

        private static IEnumerable<string> ParseChildren(XElement element)
        {
            if (element == null || element.IsEmpty) return Array.Empty<string>();

            return element.Elements("id").Select(x => x.Value);
        }

        private static MasterDataAttribute ParseAttribute(XElement element)
        {
            var attribute = new MasterDataAttribute
            {
                Id = element.Attribute("id").Value,
                Value = element.HasElements ? null : element.Value
            };

            attribute.Fields.AddRange(element.Elements().Select(ParseField));

            return attribute;
        }

        private static MasterDataField ParseField(XElement element)
        {
            var field = new MasterDataField
            {
                Value = element.HasElements ? element.Value : null,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName
            };

            field.Children.AddRange(element.Elements().Select(ParseField));

            return field;
        }
    }
}
