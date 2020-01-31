using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using FasTnT.Parsers.Xml.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class SoapResponseFormatter : BaseXmlFormatter
    {
        private XDocument FormatSoap(XElement entity)
        {
            var document = new XDocument(new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "epcisq", "urn:epcglobal:epcis-query:xsd:1")
            ));
            document.Root.Add(new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"), entity));

            return document;
        }

        public override XDocument FormatInternal(PollResponse response)
        {
            var resultName = "EventList";
            var resultList = default(IEnumerable<XElement>);

            if (response.EventList.Any())
            {
                resultName = "EventList";
                resultList = XmlEntityFormatter.FormatEvents(response.EventList);
            }
            else if (response.MasterdataList.Any())
            {
                resultName = "VocabularyList";
                resultList = XmlEntityFormatter.FormatMasterData(response.MasterdataList);
            }

            var evt = new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );

            return FormatSoap(evt);
        }

        public override XDocument FormatInternal(GetVendorVersionResponse response)
            => FormatSoap(new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version));

        public override XDocument FormatInternal(GetStandardVersionResponse response)
            => FormatSoap(new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version));

        public override XDocument FormatInternal(GetQueryNamesResponse response)
            => FormatSoap(new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x))));

        public override XDocument FormatInternal(GetSubscriptionIdsResponse response)
            => FormatSoap(new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x))));

        public override XDocument FormatInternal(ExceptionResponse response)
            => FormatSoap(new XElement(response.Exception, !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null, (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null));
    }
}
