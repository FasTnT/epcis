using FasTnT.Formatters.Xml.Formatters.Events;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    public class XmlEventFormatter
    { 
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public XElement Format(EpcisEvent epcisEvent)
        {
            if(epcisEvent.Type == EventType.Object)
                return new XmlObjectEventFormatter().Process(epcisEvent);
            else if(epcisEvent.Type == EventType.Transaction)
                return new XmlTransactionEventFormatter().Process(epcisEvent);
            else if (epcisEvent.Type == EventType.Aggregation)
                return new XmlAggregationEventFormatter().Process(epcisEvent);
            else if (epcisEvent.Type == EventType.Quantity)
                return new XmlQuantityEventFormatter().Process(epcisEvent);
            else if (epcisEvent.Type == EventType.Transformation)
                return new XmlTransformationEventFormatter().Process(epcisEvent);
            else
                throw new NotImplementedException();
        }

        public static XElement CreateEvent(string eventType, EpcisEvent @event)
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
    }
}
