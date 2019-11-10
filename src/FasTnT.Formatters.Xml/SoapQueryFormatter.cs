using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Xml
{
    public class SoapQueryFormatter
    {
        static readonly string SoapEnvelopNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        static IDictionary<string, Func<XElement, EpcisQuery>> Parsers = new Dictionary<string, Func<XElement, EpcisQuery>>
        {
            { "GetQueryNames", element => new GetQueryNames() },
            { "GetSubscriptionIDs", element => new GetSubscriptionIds { QueryName = element.Element("queryName").Value } },
            { "GetStandardVersion", element => new GetStandardVersion() },
            { "GetVendorVersion", element => new GetVendorVersion() },
            { "Poll", element => XmlQueryParser.Parse(element) },
            { "Subscribe", element => XmlSubscriptionParser.ParseSubscription(element) },
            { "Unsubscribe", element => XmlSubscriptionParser.ParseUnsubscription(element) }
        };

        public async Task<EpcisQuery> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(input, LoadOptions.None, cancellationToken);
            var body = document.Element(XName.Get("Envelope", SoapEnvelopNamespace))?.Element(XName.Get("Body", SoapEnvelopNamespace));

            if(body == null || !body.HasElements)
            {
                throw new Exception("Malformed SOAP request");
            }

            return ReadRequest(body);
        }

        private EpcisQuery ReadRequest(XElement body)
        {
            var element = body.Elements().SingleOrDefault(x => x.Name.NamespaceName == EpcisNamespaces.Query);

            if (element != null)
            {
                if (Parsers.TryGetValue(element.Name.LocalName, out Func<XElement, EpcisQuery> parserMethod))
                {
                    return parserMethod(element);
                }
                else
                {
                    throw new Exception($"Element not expected: '{element.Name.LocalName ?? null}'");
                }
            }

            throw new Exception($"Invalid SOAP request: empty Body.");
        }
    }
}
