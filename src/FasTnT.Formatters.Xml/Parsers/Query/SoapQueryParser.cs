using FasTnT.Commands.Requests;
using FasTnT.Domain.Commands;
using FasTnT.Parsers.Xml.Query;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Parsers.Query
{
    public class SoapQueryParser
    {
        const string SoapEnvelopNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        static readonly IDictionary<string, Func<XElement, IQueryRequest>> Parsers = new Dictionary<string, Func<XElement, IQueryRequest>>
        {
            { "GetQueryNames", element => new GetQueryNamesRequest() },
            { "GetSubscriptionIDs", element => new GetSubscriptionIdsRequest { QueryName = element.Element("queryName").Value } },
            { "GetStandardVersion", element => new GetStandardVersionRequest() },
            { "GetVendorVersion", element => new GetVendorVersionRequest() },
            { "Poll", element => XmlPollQueryParser.Parse(element) },
            { "Subscribe", element => XmlSubscriptionParser.ParseSubscription(element) },
            { "Unsubscribe", element => XmlSubscriptionParser.ParseUnsubscription(element) }
        };

        public async Task<IQueryRequest> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
            var body = document.Element(XName.Get("Envelope", SoapEnvelopNamespace))?.Element(XName.Get("Body", SoapEnvelopNamespace));

            if (body == null || !body.HasElements)
            {
                throw new Exception("Malformed SOAP request");
            }
            else
            {
                return DispatchElement(body.Elements().SingleOrDefault(x => x.Name.NamespaceName == EpcisNamespaces.Query));
            }
        }

        internal static IQueryRequest DispatchElement(XElement element)
        {
            if (element != null)
            {
                if (Parsers.TryGetValue(element.Name.LocalName, out Func<XElement, IQueryRequest> parseMethod))
                {
                    return parseMethod(element);
                }
                else
                {
                    throw new Exception($"Element not expected: '{element.Name.LocalName ?? null}'");
                }
            }

            throw new Exception($"EPCISBody element must contain the query type.");
        }
    }
}
