using FasTnT.Formatters.Xml.Parsers.Capture.Events;
using FasTnT.Model;
using FasTnT.Model.Enums;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Capture
{
    public static class XmlEpcisDocumentParser
    {
        public static EpcisRequest Parse(XElement root)
        {
            var request = new EpcisRequest
            {
                DocumentTime = DateTime.Parse(root.Attribute("creationDate").Value),
                SchemaVersion = root.Attribute("schemaVersion").Value
            };

            ParseHeaderIntoRequest(root.Element("EPCISHeader"), request);
            ParseBodyIntoRequest(root.Element("EPCISBody"), request);

            return request;
        }

        private static void ParseHeaderIntoRequest(XElement epcisHeader, EpcisRequest request)
        {
            if (epcisHeader == null || epcisHeader.IsEmpty) return;

            var headerMasterdata = epcisHeader.Element("extension")?.Element("EPCISMasterData")?.Element("VocabularyList");

            if (headerMasterdata != null)
            {
                request.MasterdataList.AddRange(XmlMasterdataParser.ParseMasterdata(headerMasterdata));
            }
        }

        private static void ParseBodyIntoRequest(XElement epcisBody, EpcisRequest request)
        {
            var element = epcisBody.Elements().First();

            switch (element.Name.LocalName)
            {
                case "QueryResults":
                    ParseCallbackResult(element, request);
                    break;
                case "QueryTooLargeException":
                    ParseCallbackError(element, QueryCallbackType.QueryTooLargeException, request);
                    break;
                case "ImplementationException":
                    ParseCallbackError(element, QueryCallbackType.ImplementationException, request);
                    break;
                case "EventList":
                    request.EventList.AddRange(XmlEventParser.ParseEvents(element));
                    break;
                case "VocabularyList":
                    request.MasterdataList.AddRange(XmlMasterdataParser.ParseMasterdata(element));
                    break;
            }
        }

        private static void ParseCallbackResult(XElement queryResults, EpcisRequest request)
        {
            var subscriptionId = queryResults.Element("subscriptionID")?.Value;
            var eventList = queryResults.Element("resultsBody").Element("EventList");

            request.EventList.AddRange(XmlEventParser.ParseEvents(eventList));
            request.SubscriptionCallback = new SubscriptionCallback
            {
                CallbackType = QueryCallbackType.Success,
                SubscriptionId = subscriptionId
            };
        }

        private static void ParseCallbackError(XElement element, QueryCallbackType errorType, EpcisRequest request)
        {
            request.SubscriptionCallback = new SubscriptionCallback
            {
                CallbackType = errorType,
                Reason = element.Element("reason")?.Value,
                SubscriptionId = element.Element("subscriptionID")?.Value
            };
        }
    }
}
