using System;
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
                if (element.Name == XName.Get("GetQueryNames", EpcisNamespaces.Query))
                {
                    return new GetQueryNames();
                }
                if (element.Name == XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query))
                {
                    return new GetSubscriptionIds { QueryName = element.Element("queryName").Value };
                }
                if (element.Name == XName.Get("GetStandardVersion", EpcisNamespaces.Query))
                {
                    return new GetStandardVersion();
                }
                if (element.Name == XName.Get("GetVendorVersion", EpcisNamespaces.Query))
                {
                    return new GetVendorVersion();
                }
                if (element.Name == XName.Get("Poll", EpcisNamespaces.Query))
                {
                    return XmlQueryParser.Parse(element);
                }
                if (element.Name == XName.Get("Subscribe", EpcisNamespaces.Query))
                {
                    return XmlSubscriptionParser.ParseSubscription(element);
                }
                if (element.Name == XName.Get("Unsubscribe", EpcisNamespaces.Query))
                {
                    return XmlSubscriptionParser.ParseUnsubscription(element);
                }
                throw new Exception($"Element not expected: '{element?.Name?.LocalName ?? null}'");
            }

            throw new Exception($"Invalid SOAP request: empty Body.");
        }
    }
}
