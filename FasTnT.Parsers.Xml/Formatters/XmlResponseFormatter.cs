﻿using FasTnT.Commands.Responses;
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
    public class XmlResponseFormatter
    {
        private const SaveOptions Options = SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces;

        public async Task Write(IEpcisResponse entity, Stream output, CancellationToken cancellationToken)
        {
            if (entity == default(IEpcisResponse)) return;
            
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
                case PollResponse pollResponse:
                    return FormatInternal(pollResponse);
                default:
                    throw new NotImplementedException($"Unable to format '{entity.GetType()}'");
            }
        }

        protected XDocument FormatInternal(PollResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
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

            formatted.Root.Add(
                new XElement("EPCISBody",
                    new XElement(XName.Get("QueryResults", EpcisNamespaces.Query),
                        new XElement("queryName", response.QueryName),
                        !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                        new XElement("resultsBody", new XElement(resultName, resultList))
                    )
                )
            );

            return formatted;
        }

        protected XDocument FormatInternal(GetVendorVersionResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(XName.Get("GetVendorVersionResult", EpcisNamespaces.Query), response.Version)));

            return formatted;
        }

        protected XDocument FormatInternal(GetStandardVersionResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(new XElement("EPCISBody", new XElement(XName.Get("GetStandardVersionResult", EpcisNamespaces.Query), response.Version)));

            return formatted;
        }

        protected XDocument FormatInternal(GetQueryNamesResponse response)
        {
            var formatted = CreateResponse("EPCISQueryDocument");
            formatted.Root.Add(
                new XElement("EPCISBody", new XElement(XName.Get("GetQueryNamesResult", EpcisNamespaces.Query), response.QueryNames.Select(x => new XElement("string", x))))
            );

            return formatted;
        }

        //protected XDocument FormatInternal(GetSubscriptionIdsResult response)
        //{
        //    var formatted = CreateResponse("EPCISQueryDocument");
        //    formatted.Root.Add(
        //        new XElement("EPCISBody", new XElement(XName.Get("GetSubscriptionIDsResult", EpcisNamespaces.Query), response.SubscriptionIds?.Select(x => new XElement("string", x))))
        //    );

        //    return formatted;
        //}

        protected XDocument FormatInternal(ExceptionResponse response)
        {
            var formatted = CreateResponse(response.Exception, false);
            formatted.Root.Add(!string.IsNullOrEmpty(response.Reason) ? new XElement("reason", response.Reason) : null);
            if (response.Severity != null) formatted.Root.Add(new XElement("severity", response.Severity.DisplayName));

            return formatted;
        }

        public static XDocument CreateResponse(string name, bool withAttributes = true) => new XDocument(new XElement(XName.Get(name, EpcisNamespaces.Query), new XAttribute(XNamespace.Xmlns + "epcisq", EpcisNamespaces.Query), withAttributes ? Attributes() : null));
        public static XAttribute[] Attributes() => new[] { new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1") };
    }
}