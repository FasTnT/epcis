using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Formatters.Xml.Validation;
using FasTnT.Model.Queries;

namespace FasTnT.Formatters.Xml
{
    public class XmlQueryFormatter : XmlEpcisWriter<EpcisQuery>
    {
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

        public virtual async Task<EpcisQuery> Read(Stream input, CancellationToken cancellationToken)
        {
            var document = await XmlDocumentParser.Instance.Load(input, cancellationToken);

            if (document.Root.Name.LocalName == "EPCISQueryDocument")
            {
                var element = document.Root.Element("EPCISBody").Elements().FirstOrDefault();

                return DispatchElement(element);
            }

            throw new Exception($"Element not expected: '{document.Root.Name.LocalName}'");
        }

        internal static EpcisQuery DispatchElement(XElement element)
        {
            if (element != null)
            {
                if (Parsers.TryGetValue(element.Name.LocalName, out Func<XElement, EpcisQuery> parseMethod))
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

        public override async Task Write(EpcisQuery entity, Stream output, CancellationToken cancellationToken)
        {
            await Write(entity, output, e => Format(e), cancellationToken);
        }

        private XDocument Format(GetQueryNames query) => Query(XName.Get("GetQueryNames", EpcisNamespaces.Query));
        private XDocument Format(GetSubscriptionIds query) => Query(XName.Get("GetSubscriptionIDs", EpcisNamespaces.Query));
        private XDocument Format(GetStandardVersion query) => Query(XName.Get("GetStandardVersion", EpcisNamespaces.Query));
        private XDocument Format(GetVendorVersion query) => Query(XName.Get("GetVendorVersion", EpcisNamespaces.Query));
        private XDocument Format(Poll query) => throw new NotImplementedException();

        private XDocument Query(XName queryName) => new XDocument(XName.Get("EPCISQueryDocument", EpcisNamespaces.Query), new XElement("EPCISBody", new XElement(queryName)));
    }
}
