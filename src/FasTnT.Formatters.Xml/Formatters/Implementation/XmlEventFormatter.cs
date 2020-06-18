using FasTnT.Formatters.Xml.Formatters.Events;
using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters.Implementation
{

    public static class XmlEventFormatter
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static IDictionary<EventType, Func<EpcisEvent, XElement>> Formatters = new Dictionary<EventType, Func<EpcisEvent, XElement>>
        {
            { EventType.Object,         evt => new XmlObjectEventFormatter().Process(evt) },
            { EventType.Transaction,    evt => new XmlTransactionEventFormatter().Process(evt) },
            { EventType.Aggregation,    evt => new XmlAggregationEventFormatter().Process(evt) },
            { EventType.Quantity,       evt => new XmlQuantityEventFormatter().Process(evt) },
            { EventType.Transformation, evt => new XmlTransformationEventFormatter().Process(evt) }
        };

        public static XElement Format(EpcisEvent epcisEvent)
        {
            if (Formatters.TryGetValue(epcisEvent.Type, out Func<EpcisEvent, XElement> formatter))
            {
                return formatter(epcisEvent);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IEnumerable<XElement> FormatEpcQuantity(EpcisEvent evt, EpcType epcType)
        {
            return evt.Epcs.Where(x => x.Type == epcType).Select(FormatQuantity);
        }

        internal static IEnumerable<XElement> FormatEpcList(EpcisEvent evt, EpcType epcType)
        {
            return evt.Epcs.Where(x => x.Type == epcType).Select(e => new XElement("epc", e.Id));
        }

        internal static XElement CreateEvent(string eventType, EpcisEvent @event)
        {
            var element = new XElement(eventType);

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));
            if (@event.CorrectiveDeclarationTime.HasValue || !string.IsNullOrEmpty(@event.EventId)) AddErrorDeclaration(@event, element);

            return element;
        }
        private static void AddErrorDeclaration(EpcisEvent @event, XElement element)
        {
            XElement errorDeclaration = default, eventId = default;

            if (@event.CorrectiveDeclarationTime.HasValue)
            {
                var correctiveEventIds = @event.CorrectiveEventIds.Any() ? new XElement("correctiveEventIDs", @event.CorrectiveEventIds.Select(x => new XElement("correctiveEventID", x))) : null;
                errorDeclaration = new XElement("errorDeclaration", new XElement("declarationTime", @event.CorrectiveDeclarationTime), new XElement("reason", @event.CorrectiveReason), correctiveEventIds);
            }

            if (!string.IsNullOrEmpty(@event.EventId))
            {
                eventId = new XElement("eventID", @event.EventId);
            }

            element.Add(new XElement("baseExtension", eventId, errorDeclaration));
        }

        internal static IEnumerable<XElement> GenerateSourceDest(EpcisEvent evt)
        {
            if (evt.SourceDestinationList == null || !evt.SourceDestinationList.Any())
            {
                return default;
            }

            var source = new XElement("sourceList");
            var destination = new XElement("destinationList");

            foreach (var sourceDest in evt.SourceDestinationList)
            {
                if (sourceDest.Direction == SourceDestinationType.Source)
                {
                    source.Add(new XElement("source", new XAttribute("type", sourceDest.Type), sourceDest.Id));
                }
                else if (sourceDest.Direction == SourceDestinationType.Destination)
                {
                    destination.Add(new XElement("destination", new XAttribute("type", sourceDest.Type), sourceDest.Id));
                }
            }

            return new[] { source, destination };
        }

        internal static XElement GenerateBusinessTransactions(EpcisEvent evt)
        {
            var businessTransactions = default(XElement);

            if (evt.BusinessTransactions != null && evt.BusinessTransactions.Any())
            {
                businessTransactions = new XElement("bizTransactionList");

                foreach (var trans in evt.BusinessTransactions)
                {
                    businessTransactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));
                }
            }

            return businessTransactions;
        }

        internal static XElement GenerateReadPoint(EpcisEvent evt)
        {
            var readPoint = default(XElement);

            if (!string.IsNullOrEmpty(evt.ReadPoint))
            {
                readPoint = new XElement("readPoint", new XElement("id", evt.ReadPoint)/* TODO , GenerateCustomFields(evt, FieldType.ReadPointExtension) */);
            }

            return readPoint;
        }

        internal static XElement GenerateDisposition(EpcisEvent evt) => SimpleElement("disposition", evt.Disposition);
        internal static XElement GenerateBusinesStep(EpcisEvent evt) => SimpleElement("bizStep", evt.BusinessStep);

        internal static XElement GenerateBusinessLocation(EpcisEvent evt)
        {
            var businessLocation = default(XElement);

            if (!string.IsNullOrEmpty(evt.BusinessLocation))
            {
                businessLocation = new XElement("bizLocation", new XElement("id", evt.BusinessLocation)/* TODO , GenerateCustomFields(evt, FieldType.BusinessLocationExtension) */); ;
            }

            return businessLocation;
        }

        internal static IEnumerable<XElement> GenerateCustomFields(EpcisEvent evt, FieldType type)
        {
            var container = new XElement("container");

            foreach (var field in evt.CustomFields.Where(x => x.Type == type))
            {
                GenerateCustomFields(type, container, field);
            }

            return container.Elements();
        }

        private static XElement SimpleElement(string elementName, string value)
        {
            var element = default(XElement);

            if (!string.IsNullOrEmpty(value))
            {
                element = new XElement(elementName, value);
            }

            return element;
        }

        private static void InnerCustomFields(XContainer element, FieldType type, CustomField parent)
        {
            foreach (var field in parent.Children.Where(x => x.Type == type))
            {
                GenerateCustomFields(type, element, field);
            }
        }

        private static XElement FormatQuantity(Epc epc)
        {
            var qtyElement = new XElement("quantityElement");
            qtyElement.Add(new XElement("epcClass", epc.Id));
            if (epc.Quantity.HasValue) qtyElement.Add(new XElement("quantity", epc.Quantity));
            if (!string.IsNullOrEmpty(epc.UnitOfMeasure)) qtyElement.Add(new XElement("uom", epc.UnitOfMeasure));

            return qtyElement;
        }

        private static void GenerateCustomFields(FieldType type, XContainer container, CustomField rootField)
        {
            var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

            InnerCustomFields(xmlElement, type, rootField);
            foreach (var attribute in rootField.Children.Where(x => x.Type == FieldType.Attribute))
            {
                xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
            }

            container.Add(xmlElement);
        }
    }
}
