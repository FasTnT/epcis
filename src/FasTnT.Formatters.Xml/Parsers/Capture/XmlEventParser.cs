using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Parsers.Capture.Events
{
    public static class XmlEventParser
    {
        readonly static IDictionary<string, Func<XElement, EpcisEvent>> RootParsers = new Dictionary<string, Func<XElement, EpcisEvent>>
        {
            { "ObjectEvent", ParseObjectEvent },
            { "TransactionEvent", ParseTransactionEvent },
            { "AggregationEvent", ParseAggregationEvent },
            { "QuantityEvent", ParseQuantityEvent },
            { "extension", ParseEventListExtension }
        };

        readonly static IDictionary<string, Func<XElement, EpcisEvent>> ExtensionParsers = new Dictionary<string, Func<XElement, EpcisEvent>>
        {
            { "TransformationEvent", ParseTransformationEvent }
        };

        public static IEnumerable<EpcisEvent> ParseEvents(XElement root)
        {
            foreach (var element in root.Elements())
            {
                if (RootParsers.TryGetValue(element.Name.LocalName, out Func<XElement, EpcisEvent> parser))
                {
                    yield return parser(element);
                }
                else
                {
                    throw new Exception($"Element '{element.Name.LocalName}' not expected in this context");
                }
            }
        }

        private static EpcisEvent ParseEventListExtension(XElement element)
        {
            var eventElement = element.Elements().First();

            if (ExtensionParsers.TryGetValue(eventElement.Name.LocalName, out Func<XElement, EpcisEvent> parser))
            {
                return parser(eventElement);
            }
            else
            {
                throw new Exception($"Element '{eventElement.Name.LocalName}' not expected in this context");
            }
        }

        public static EpcisEvent ParseObjectEvent(XElement eventRoot)
        {
            var epcisEvent = ParseBase(eventRoot, EventType.Object);

            epcisEvent.Action = Enumeration.GetByDisplayName<EventAction>(eventRoot.Element("action").Value);
            ParseTransactions(eventRoot.Element("bizTransactionList"), epcisEvent);
            ParseEpcList(eventRoot.Element("epcList"), epcisEvent, EpcType.List);
            ParseObjectExtension(eventRoot.Element("extension"), epcisEvent);

            return epcisEvent;
        }

        private static void ParseObjectExtension(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            ParseEpcQuantityList(element.Element("quantityList"), epcisEvent, EpcType.Quantity);
            ParseSources(element.Element("sourceList"), epcisEvent);
            ParseDestinations(element.Element("destinationList"), epcisEvent);
            ParseIlmd(element.Element("ilmd"), epcisEvent);
            ParseExtension(element.Element("extension"), epcisEvent, FieldType.Extension);
        }

        public static EpcisEvent ParseAggregationEvent(XElement eventRoot)
        {
            var epcisEvent = ParseBase(eventRoot, EventType.Aggregation);

            epcisEvent.Action = Enumeration.GetByDisplayName<EventAction>(eventRoot.Element("action").Value);
            ParseParentId(eventRoot.Element("parentID"), epcisEvent);
            ParseEpcList(eventRoot.Element("childEPCs"), epcisEvent, EpcType.ChildEpc);
            ParseTransactions(eventRoot.Element("bizTransactionList"), epcisEvent);
            ParseAggregationExtension(eventRoot.Element("extension"), epcisEvent);

            return epcisEvent;
        }

        private static void ParseAggregationExtension(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            ParseEpcQuantityList(element.Element("childQuantityList"), epcisEvent, EpcType.ChildQuantity);
            ParseSources(element.Element("sourceList"), epcisEvent);
            ParseDestinations(element.Element("destinationList"), epcisEvent);
            ParseExtension(element.Element("extension"), epcisEvent, FieldType.Extension);
        }

        public static EpcisEvent ParseTransactionEvent(XElement eventRoot)
        {
            var epcisEvent = ParseBase(eventRoot, EventType.Transaction);

            epcisEvent.Action = Enumeration.GetByDisplayName<EventAction>(eventRoot.Element("action").Value);
            ParseParentId(eventRoot.Element("parentID"), epcisEvent);
            ParseTransactions(eventRoot.Element("bizTransactionList"), epcisEvent);
            ParseEpcList(eventRoot.Element("epcList"), epcisEvent, EpcType.List);
            ParseTransactionExtension(eventRoot.Element("extension"), epcisEvent);

            return epcisEvent;
        }

        private static void ParseTransactionExtension(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            ParseEpcQuantityList(element.Element("quantityList"), epcisEvent, EpcType.Quantity);
            ParseSources(element.Element("sourceList"), epcisEvent);
            ParseDestinations(element.Element("destinationList"), epcisEvent);
            ParseFields(element, epcisEvent, FieldType.Extension);
        }

        public static EpcisEvent ParseQuantityEvent(XElement eventRoot)
        {
            var epcisEvent = ParseBase(eventRoot, EventType.Quantity);
            var epcQuantity = new Epc
            {
                Id = eventRoot.Element("epcClass").Value,
                Quantity = float.Parse(eventRoot.Element("quantity").Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB")),
                IsQuantity = true,
                Type = EpcType.Quantity
            };

            epcisEvent.Epcs.Add(epcQuantity);
            ParseExtension(eventRoot.Element("extension"), epcisEvent, FieldType.Extension);

            return epcisEvent;
        }

        public static EpcisEvent ParseTransformationEvent(XElement eventRoot)
        {
            var epcisEvent = ParseBase(eventRoot, EventType.Transformation);

            epcisEvent.TransformationId = eventRoot.Element("transformationID")?.Value;
            ParseTransactions(eventRoot.Element("bizTransactionList"), epcisEvent);
            ParseSources(eventRoot.Element("sourceList"), epcisEvent);
            ParseDestinations(eventRoot.Element("destinationList"), epcisEvent);
            ParseIlmd(eventRoot.Element("ilmd"), epcisEvent);
            ParseEpcList(eventRoot.Element("inputEPCList"), epcisEvent, EpcType.InputEpc);
            ParseEpcQuantityList(eventRoot.Element("inputQuantityList"), epcisEvent, EpcType.InputQuantity);
            ParseEpcList(eventRoot.Element("outputEPCList"), epcisEvent, EpcType.OutputEpc);
            ParseEpcQuantityList(eventRoot.Element("outputQuantityList"), epcisEvent, EpcType.OutputQuantity);
            ParseExtension(eventRoot.Element("extension"), epcisEvent, FieldType.Extension);

            return epcisEvent;
        }

        public static EpcisEvent ParseBase(XElement eventRoot, EventType eventType)
        {
            var epcisEvent = new EpcisEvent
            {
                Type = eventType,
                EventTime = DateTime.Parse(eventRoot.Element("eventTime").Value),
                EventTimeZoneOffset = new TimeZoneOffset { Representation = eventRoot.Element("eventTimeZoneOffset").Value },
                BusinessStep = eventRoot.Element("bizStep")?.Value,
                Disposition = eventRoot.Element("disposition")?.Value,
            };

            ParseReadPoint(eventRoot.Element("readPoint"), epcisEvent);
            ParseBusinessLocation(eventRoot.Element("bizLocation"), epcisEvent);
            ParseBaseExtension(eventRoot.Element("baseExtension"), epcisEvent);
            ParseFields(eventRoot, epcisEvent, FieldType.CustomField);

            return epcisEvent;
        }

        private static void ParseReadPoint(XElement readPoint, EpcisEvent epcisEvent)
        {
            if (readPoint == null || readPoint.IsEmpty) return;

            epcisEvent.ReadPoint = readPoint.Element("id")?.Value;
            ParseExtension(readPoint.Element("extension"), epcisEvent, FieldType.ReadPointExtension);
            ParseFields(readPoint, epcisEvent, FieldType.ReadPointCustomField);
        }

        private static void ParseBusinessLocation(XElement bizLocation, EpcisEvent epcisEvent)
        {
            if (bizLocation == null || bizLocation.IsEmpty) return;

            epcisEvent.BusinessLocation = bizLocation.Element("id")?.Value;
            ParseExtension(bizLocation.Element("extension"), epcisEvent, FieldType.BusinessLocationExtension);
            ParseFields(bizLocation, epcisEvent, FieldType.BusinessLocationCustomField);
        }

        private static void ParseParentId(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.Epcs.Add(new Epc { Id = element.Value, Type = EpcType.ParentId });
        }

        private static void ParseIlmd(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            ParseFields(element, epcisEvent, FieldType.Ilmd);
            ParseExtension(element.Element("extension"), epcisEvent, FieldType.IlmdExtension);
        }

        private static void ParseEpcList(XElement element, EpcisEvent epcisEvent, EpcType type)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.Epcs.AddRange(element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type }));
        }

        private static void ParseEpcQuantityList(XElement element, EpcisEvent epcisEvent, EpcType type)
        {
            if (element == null || element.IsEmpty) return; 

            epcisEvent.Epcs.AddRange(element.Elements("quantityElement").Select(x => new Epc
            {
                Id = x.Element("epcClass").Value,
                IsQuantity = true,
                Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
                UnitOfMeasure = x.Element("uom")?.Value,
                Type = type
            }));
        }

        private static void ParseBaseExtension(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.EventId = element.Element("eventID")?.Value;
            ParseErrorDeclaration(element.Element("errorDeclaration"), epcisEvent);
            ParseExtension(element.Element("extension"), epcisEvent, FieldType.BaseExtension);
        }

        private static void ParseErrorDeclaration(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.CorrectiveDeclarationTime = DateTime.Parse(element.Element("declarationTime").Value);
            epcisEvent.CorrectiveReason = element.Element("reason")?.Value;
            epcisEvent.CorrectiveEventIds.AddRange(element.Element("correctiveEventIDs")?.Elements("correctiveEventID")?.Select(x => x.Value));
            ParseExtension(element.Element("extension"), epcisEvent, FieldType.ErrorDeclarationExtension);
            ParseFields(element, epcisEvent, FieldType.ErrorDeclarationCustomField);
        }

        internal static void ParseFields(XElement element, EpcisEvent epcisEvent, FieldType type)
        {
            if (element == null || element.IsEmpty) return;

            var customFields = element.Elements().Where(x => !string.IsNullOrEmpty(x.Name.NamespaceName));
            epcisEvent.CustomFields.AddRange(customFields.Select(x => ParseCustomFields(x, type)));
        }

        internal static void ParseExtension(XElement element, EpcisEvent epcisEvent, FieldType type)
        {
            if (element == null || element.IsEmpty) return;
            
            var customFields = element.Elements().Where(x => string.IsNullOrEmpty(x.Name.NamespaceName));
            epcisEvent.CustomFields.AddRange(customFields.Select(x => ParseCustomFields(x, type)));
        }

        internal static void ParseSources(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.SourceDestinationList.AddRange(element.Elements("source").Select(x => CreateSourceDest(x, SourceDestinationType.Source)));
        }

        internal static void ParseDestinations(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.SourceDestinationList.AddRange(element.Elements("destination").Select(x => CreateSourceDest(x, SourceDestinationType.Destination)));
        }

        internal static void ParseTransactions(XElement element, EpcisEvent epcisEvent)
        {
            if (element == null || element.IsEmpty) return;

            epcisEvent.BusinessTransactions.AddRange(element.Elements("bizTransaction").Select(CreateBusinessTransaction));
        }

        private static BusinessTransaction CreateBusinessTransaction(XElement element)
        {
            return new BusinessTransaction
            {
                Id = element.Value,
                Type = element.Attribute("type").Value
            };
        }

        private static SourceDestination CreateSourceDest(XElement element, SourceDestinationType destination)
        {
            return new SourceDestination
            {
                Type = element.Attribute("type").Value,
                Id = element.Value,
                Direction = destination
            };
        }

        public static CustomField ParseCustomFields(XElement element, FieldType fieldType)
        {
            var field = new CustomField
            {
                Type = fieldType,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                TextValue = element.HasElements ? default : element.Value,
                NumericValue = element.HasElements ? default : float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
                DateValue = element.HasElements ? default : DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
            };

            field.Children.AddRange(element.Elements().Select(x => ParseCustomFields(x, fieldType)));
            field.Children.AddRange(element.Attributes().Where(x => !x.IsNamespaceDeclaration).Select(ParseAttribute));

            return field;
        }

        public static CustomField ParseAttribute(XAttribute element)
        {
            return new CustomField
            {
                Type = FieldType.Attribute,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                TextValue = element.Value,
                NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
                DateValue = DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
            };
        }
    }
}
