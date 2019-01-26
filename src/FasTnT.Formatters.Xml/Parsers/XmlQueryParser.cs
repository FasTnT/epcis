using FasTnT.Model.Queries;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace FasTnT.Formatters.Xml.Requests
{
    public static class XmlQueryParser
    {
        internal static Poll Parse(XElement element)
        {
            var queryName = element.Element("queryName").Value;
            var parameters = ParseParameters(element.Element("params")?.Elements()).ToArray();

            return new Poll
            {
                QueryName = queryName,
                Parameters = parameters
            };
        }

        internal static IEnumerable<QueryParameter> ParseParameters(IEnumerable<XElement> elements)
        {
            foreach (var element in elements ?? new XElement[0])
            {
                var name = element.Element("name")?.Value?.Trim();
                var values = element.Element("value")?.HasElements ?? false ? element.Element("value").Elements("value").Select(x => x.Value) : new[] { element.Element("value")?.Value };

                yield return new QueryParameter
                {
                    Name = name,
                    Values = values.ToArray()
                };
            }
        }
    }
}
