using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Formatters.Xml.Requests
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

        private static IEnumerable<EpcisMasterData> ParseVocabularyElements(string type, IEnumerable<XElement> elements)
        {
            return elements.Select(e => 
            {
                var masterData = new EpcisMasterData { Id = e.Attribute("id").Value, Type = type };
                masterData.Attributes = ParseAttributes(e.Elements("attribute"), masterData).ToList();

                return masterData;
            });
        }

        private static IEnumerable<MasterDataAttribute> ParseAttributes(IEnumerable<XElement> elements, EpcisMasterData masterData)
        {
            if (elements.Any(x => x.HasElements)) throw new NotImplementedException("Inner CBV properties is not implemented yet.");
            return elements.Select(a => new MasterDataAttribute
            {
                ParentId = masterData.Id,
                ParentType = masterData.Type,
                Id = a.Attribute("id").Value,
                Value = a.Value
            });
        }
    }
}
