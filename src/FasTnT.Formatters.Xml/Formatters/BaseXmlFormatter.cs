using FasTnT.Commands.Responses;
using FasTnT.Parsers.Xml.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public abstract class BaseXmlFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

        public async Task Write(IEpcisResponse response, Stream output, CancellationToken cancellationToken)
        {
            if (response == default || response is EmptyResponse) return;

            var formattedResponse = Format(response);
            var wrappedResponse = WrapResponse(formattedResponse);

            await wrappedResponse.SaveAsync(output, Options, cancellationToken);
        }

        private XElement Format(IEpcisResponse entity)
        {
            return entity switch
            {
                GetStandardVersionResponse standardVersion => FormatGetStandardVersionResponse(standardVersion),
                GetVendorVersionResponse vendorVersion => FormatGetVendorVersionResponse(vendorVersion),
                ExceptionResponse exception => FormatInternal(exception),
                GetQueryNamesResponse queryNames => FormatGetQueryNamesResponse(queryNames),
                GetSubscriptionIdsResponse subscriptionIds => FormatGetSubscriptionIdsResponse(subscriptionIds),
                PollResponse poll => FormatPollResponse(poll),
                _ => throw new NotImplementedException($"Unable to format '{entity.GetType()}'")
            };
        }

        internal abstract XDocument WrapResponse(XElement response);

        private XElement FormatGetStandardVersionResponse(GetStandardVersionResponse response)
        {
            return new XElement(ElementName("GetStandardVersionResult"), response.Version);
        }

        private XElement FormatGetVendorVersionResponse(GetVendorVersionResponse response)
        {
            return new XElement(ElementName("GetVendorVersionResult"), response.Version);
        }

        private XElement FormatGetQueryNamesResponse(GetQueryNamesResponse response)
        {
            return new XElement(ElementName("GetQueryNamesResult"), response.QueryNames.Select(x => new XElement("string", x)));
        }

        private XElement FormatGetSubscriptionIdsResponse(GetSubscriptionIdsResponse response)
        {
            return new XElement(ElementName("GetSubscriptionIDsResult"), response.SubscriptionIds.Select(x => new XElement("string", x))); 
        }

        private XElement FormatPollResponse(PollResponse response)
        {
            var resultName = "EventList";
            var resultList = default(IEnumerable<XElement>);

            if (response.EventList.Any())
            {
                resultName = "EventList";
                resultList = XmlEventFormatter.FormatList(response.EventList);
            }
            else if (response.MasterdataList.Any())
            {
                resultName = "VocabularyList";
                resultList = XmlMasterdataFormatter.FormatMasterData(response.MasterdataList);
            }

            var unwrappedResponse = new XElement(ElementName("QueryResults"),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );

            return unwrappedResponse;
        }

        private XElement FormatInternal(ExceptionResponse response)
        {
            var reason = !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null;
            var severity = (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null;

            return new XElement(response.Exception, reason, severity);
        }

        private XName ElementName(string localName) => XName.Get(localName, EpcisNamespaces.Query);
    }
}
