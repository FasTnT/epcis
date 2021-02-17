using FasTnT.Model.Queries;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using FasTnT.Domain.Commands;
using FasTnT.Commands.Requests;
using System;

namespace FasTnT.Parsers.Xml.Query
{
    public static class XmlPollQueryParser
    {
        internal static IQueryRequest Parse(XElement element)
        {
            var queryName = element.Element("queryName").Value;
            var parameters = ParseParameters(element.Element("params")?.Elements()).ToArray();

            return new PollRequest
            {
                QueryName = queryName,
                Parameters = parameters
            };
        }

        internal static IEnumerable<QueryParameter> ParseParameters(IEnumerable<XElement> elements)
        {
            foreach (var element in elements ?? Array.Empty<XElement>())
            {
                var name = element.Element("name")?.Value?.Trim();
                var values = element.Element("value")?.HasElements ?? false ? element.Element("value").Elements().Select(x => x.Value) : new[] { element.Element("value")?.Value };

                yield return new QueryParameter
                {
                    Name = name,
                    Values = values.ToArray()
                };
            }
        }
    }
}
