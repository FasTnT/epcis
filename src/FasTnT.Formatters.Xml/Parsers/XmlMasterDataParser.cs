using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Model.MasterDatas;

namespace FasTnT.Formatters.Xml.Requests
{
    public static class XmlMasterDataParser
    {
        internal static IList<EpcisMasterData> ParseMasterDatas(IEnumerable<XElement> elements)
        {
            var parsedElements = new List<EpcisMasterData>();

            foreach(var element in elements)
            {
                if (element.Name.LocalName == "Vocabulary")
                {
                    parsedElements.AddRange(ParseVocabularyElements(element.Attribute("type").Value, element.Element("VocabularyElementList").Elements("VocabularyElement")));
                }
                else throw new Exception($"Element not expected: '{element.Name}'");
            }

            return parsedElements;
        }

        private static IEnumerable<EpcisMasterData> ParseVocabularyElements(string type, IEnumerable<XElement> elements)
        {
            return elements.Select(x => new EpcisMasterData
            {
                Name = x.Attribute("id").Value,
                Attributes = x.Elements("attribute").Select(a => new MasterDataAttribute
                {
                    Name = a.Attribute("id").Value,
                    Value = a.Value
                }).ToList()
            });
        }
    }
}
