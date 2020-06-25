using FasTnT.Commands.Responses;
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
    public abstract class BaseXmlFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

        public async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default || entity is EmptyResponse) return;

            await Format(entity).SaveAsync(output, Options, cancellationToken);
        }

        private XDocument Format(IEpcisResponse entity)
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
                case GetSubscriptionIdsResponse getSubscriptionIdsResponse:
                    return FormatInternal(getSubscriptionIdsResponse);
                case PollResponse pollResponse:
                    return FormatInternal(pollResponse);
                default:
                    throw new NotImplementedException($"Unable to format '{entity.GetType()}'");
            }
        }

        internal abstract XDocument WrapResponse(XElement response);

        public XDocument FormatInternal(GetStandardVersionResponse response)
            => WrapResponse(new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version));

        public XDocument FormatInternal(GetVendorVersionResponse response)
            => WrapResponse(new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version));

        public XDocument FormatInternal(GetQueryNamesResponse response)
            => WrapResponse(new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x))));

        public XDocument FormatInternal(GetSubscriptionIdsResponse response)
            => WrapResponse(new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds.Select(x => new XElement("string", x))));

        public XDocument FormatInternal(PollResponse response)
            => WrapResponse(FormatPollResponse(response));

        public XDocument FormatInternal(ExceptionResponse response)
        {
            var reason = !string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null;
            var severity = (response.Severity != null) ? new XElement("severity", response.Severity.DisplayName) : null;

            return WrapResponse(new XElement(response.Exception, reason, severity));
        }

        protected static XElement FormatPollResponse(PollResponse response)
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

            return new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );
        }
    }
}
