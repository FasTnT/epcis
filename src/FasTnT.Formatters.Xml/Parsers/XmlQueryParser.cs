using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations.PredefinedQueries;
using FasTnT.Formatters.Xml.Requests.Queries;
using System.Xml.Linq;
using System.Linq;

namespace FasTnT.Formatters.Xml.Requests
{
    public static class XmlQueryParser
    {
        internal static PredefinedQuery Parse(XElement element)
        {
            var queryName = element.Element("queryName");

            if (queryName.Value == SimpleEventQuery.Name)
            {
                return new SimpleEventQuery
                {
                    Parameters = SimpleEventQueryParameterFactory.ParseParameters(element.Element("params")?.Elements()).ToArray()
                };
            }

            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName.Value}' is unknown");
        }
    }
}
