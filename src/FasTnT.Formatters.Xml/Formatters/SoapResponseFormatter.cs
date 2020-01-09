using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class SoapResponseFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

        public async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default(IEpcisResponse)) return;

            await FormatSoap(Format(entity)).SaveAsync(output, Options, cancellationToken);
        }

        private XElement Format(IEpcisResponse entity)
        {
            switch (entity)
            {
                case GetStandardVersionResponse getStandardVersionResponse:
                    return FormatInternal(getStandardVersionResponse);
                case GetVendorVersionResponse getVendorVersionResponse:
                    return FormatInternal(getVendorVersionResponse);
                case ExceptionResponse exceptionResponse:
                    return FormatInternal(exceptionResponse);
                case GetQueryNamesResponse getQueryNamesResponse:
                    return FormatInternal(getQueryNamesResponse);
                case PollResponse pollResponse:
                    return FormatInternal(pollResponse);
                default:
                    throw new NotImplementedException($"Unable to format '{entity.GetType()}'");
            }
        }

        private XDocument FormatSoap(XElement entity)
        {
            var document = new XDocument(new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "epcisq", "urn:epcglobal:epcis-query:xsd:1")
            ));
            document.Root.Add(new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"), entity));

            return document;
        }

        protected XElement FormatInternal(PollResponse response)
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

            return evt;
        }

        protected XElement FormatInternal(GetVendorVersionResponse response)
            => new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version);

        protected XElement FormatInternal(GetStandardVersionResponse response)
            => new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version);

        protected XElement FormatInternal(GetQueryNamesResponse response)
            => new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x)));

        //protected XElement FormatInternal(GetSubscriptionIdsResponse response)
        //    => new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x)));

        protected XElement FormatInternal(ExceptionResponse response)
            => new XElement(response.Exception, !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null, (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null);
    }
}
