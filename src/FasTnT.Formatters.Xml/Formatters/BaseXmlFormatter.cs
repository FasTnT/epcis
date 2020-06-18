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

        public abstract XDocument FormatInternal(GetStandardVersionResponse response);
        public abstract XDocument FormatInternal(GetVendorVersionResponse response);
        public abstract XDocument FormatInternal(ExceptionResponse response);
        public abstract XDocument FormatInternal(GetQueryNamesResponse response);
        public abstract XDocument FormatInternal(GetSubscriptionIdsResponse response);
        public abstract XDocument FormatInternal(PollResponse response);

        protected XElement FormatPollResponse(PollResponse response)
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
                //resultName = "VocabularyList";
                //resultList = XmlMasterdataFormatter.FormatMasterData(response.MasterdataList);
            }

            return new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );
        }
    }
}
