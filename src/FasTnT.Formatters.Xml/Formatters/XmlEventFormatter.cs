using FasTnT.Formatters.Xml.Formatters.Events;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    public class XmlEventFormatter
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

        public XElement Format(EpcisEvent epcisEvent)
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

        internal static XElement CreateEvent(string eventType, EpcisEvent @event)
        {
            var element = new XElement(eventType);

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));
            if (@event.ErrorDeclaration != null || !string.IsNullOrEmpty(@event.EventId)) AddErrorDeclaration(@event, element);

            return element;
        }
        private static void AddErrorDeclaration(EpcisEvent @event, XElement element)
        {
            XElement errorDeclaration = default, eventId = default;
            {
                var correctiveEventIds = @event.ErrorDeclaration.CorrectiveEventIds.Any() ? new XElement("correctiveEventIDs", @event.ErrorDeclaration.CorrectiveEventIds.Select(x => new XElement("correctiveEventID", x.CorrectiveId))) : null;
                errorDeclaration = new XElement("errorDeclaration", new XElement("declarationTime", @event.ErrorDeclaration.DeclarationTime), new XElement("reason", @event.ErrorDeclaration.Reason), correctiveEventIds);
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
                readPoint = new XElement("readPoint", new XElement("id", evt.ReadPoint));

                foreach (var ext in evt.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                {
                    readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));
                }
            }

            return readPoint;
        }

        internal static XElement GenerateDisposition(EpcisEvent evt)
        {
            var disposition = default(XElement);

            if (!string.IsNullOrEmpty(evt.Disposition))
            {
                disposition = new XElement("disposition", evt.Disposition);
            }

            return disposition;
        }

        internal static XElement GenerateBusinesStep(EpcisEvent evt)
        {
            var businessStep = default(XElement);

            if (!string.IsNullOrEmpty(evt.BusinessStep))
            {
                businessStep = new XElement("bizStep", evt.BusinessStep);
            }

            return businessStep;
        }

        internal static XElement GenerateBusinessLocation(EpcisEvent evt)
        {
            var businessLocation = default(XElement);

            if (!string.IsNullOrEmpty(evt.BusinessLocation))
            {
                var custom = evt.CustomFields.Where(x => x.Type == FieldType.BusinessLocationExtension).Select(field => new XElement(XName.Get(field.Name, field.Namespace), field.TextValue));

                businessLocation = new XElement("bizLocation", new XElement("id", evt.BusinessLocation), custom);
            }

            return businessLocation;
        }

        internal static XElement FormatQuantity(Epc epc)
        {
            var qtyElement = new XElement("quantityElement");
            qtyElement.Add(new XElement("epcClass", epc.Id));
            if (epc.Quantity.HasValue) qtyElement.Add(new XElement("quantity", epc.Quantity));
            if (!string.IsNullOrEmpty(epc.UnitOfMeasure)) qtyElement.Add(new XElement("uom", epc.UnitOfMeasure));

            return qtyElement;
        }

        internal static IEnumerable<XElement> GenerateCustomFields(EpcisEvent evt, FieldType type)
        {
            var elements = new List<XElement>();
            foreach (var rootField in evt.CustomFields.Where(x => x.Type == type))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                InnerCustomFields(xmlElement, type, rootField);
                foreach (var attribute in rootField.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                elements.Add(xmlElement);
            }

            return elements;
        }

        private static void InnerCustomFields(XContainer element, FieldType type, CustomField parent)
        {
            foreach (var field in parent.Children.Where(x => x.Type == type))
            {
                var xmlElement = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);

                InnerCustomFields(xmlElement, type, field);
                foreach (var attribute in field.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }
    }
}
