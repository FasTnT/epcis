using FasTnT.Domain;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    public class XmlEventFormatter
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public static XElement Format(IEntity entity) => Format((dynamic)entity);

        public static XElement Format(EpcisEvent epcisEvent)
        {
            if(epcisEvent.Type == EventType.Object) return FormatObjectEvent(epcisEvent);
            else if (epcisEvent.Type == EventType.Quantity) return FormatQuantityEvent(epcisEvent);
            else if (epcisEvent.Type == EventType.Aggregation) return FormatAggregationEvent(epcisEvent);
            else if (epcisEvent.Type == EventType.Transaction) return FormatTransactionEvent(epcisEvent);
            else if (epcisEvent.Type == EventType.Transformation) return FormatTransformationEvent(epcisEvent);

            throw new Exception($"Unknown event type: '{epcisEvent.Type?.DisplayName}");
        }

        private static XElement FormatObjectEvent(EpcisEvent @event)
        {
            var element = new XElement("ObjectEvent");

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));

            AddEpcList(@event, element);

            element.Add(new XElement("action", @event.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));

            if (!string.IsNullOrEmpty(@event.BusinessStep)) element.Add(new XElement("bizStep", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.Disposition)) element.Add(new XElement("disposition", @event.Disposition));

            AddReadPoint(@event, element);
            AddBusinessLocation(@event, element);
            AddBusinessTransactions(@event, element);
            AddIlmd(@event, element);
            AddSourceDest(@event, element);
            AddCustomFields(@event, element, FieldType.EventExtension);

            return element;
        }

        private static XElement FormatQuantityEvent(EpcisEvent @event)
        {
            var element = new XElement("QuantityEvent");

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));

            AddEpcList(@event, element);

            element.Add(new XElement("action", @event.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));

            if (!string.IsNullOrEmpty(@event.BusinessStep)) element.Add(new XElement("bizStep", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.Disposition)) element.Add(new XElement("disposition", @event.Disposition));

            AddReadPoint(@event, element);
            AddBusinessLocation(@event, element);
            AddBusinessTransactions(@event, element);
            AddIlmd(@event, element);
            AddSourceDest(@event, element);
            AddCustomFields(@event, element, FieldType.EventExtension);

            return element;
        }

        private static XElement FormatAggregationEvent(EpcisEvent @event)
        {
            var element = new XElement("AggregationEvent");

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));

            AddParentId(@event, element);
            AddChildEpcs(@event, element);

            element.Add(new XElement("action", @event.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));

            if (!string.IsNullOrEmpty(@event.BusinessStep)) element.Add(new XElement("bizStep", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.Disposition)) element.Add(new XElement("disposition", @event.Disposition));

            AddReadPoint(@event, element);
            AddBusinessLocation(@event, element);
            AddBusinessTransactions(@event, element);
            AddSourceDest(@event, element);
            AddCustomFields(@event, element, FieldType.EventExtension);

            return element;
        }

        private static XElement FormatTransactionEvent(EpcisEvent @event)
        {
            var element = new XElement("TransactionEvent");

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));

            AddEpcList(@event, element);

            element.Add(new XElement("action", @event.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));

            if (!string.IsNullOrEmpty(@event.BusinessStep)) element.Add(new XElement("bizStep", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.Disposition)) element.Add(new XElement("disposition", @event.Disposition));

            AddReadPoint(@event, element);
            AddBusinessLocation(@event, element);
            AddBusinessTransactions(@event, element);
            AddSourceDest(@event, element);
            AddCustomFields(@event, element, FieldType.EventExtension);

            return element;
        }

        private static XElement FormatTransformationEvent(EpcisEvent Event)
        {
            var element = new XElement("TransformationEvent");

            element.Add(new XElement("eventTime", Event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", Event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", Event.EventTimeZoneOffset.Representation));

            AddEpcList(Event, element);

            if (!string.IsNullOrEmpty(Event.TransformationId)) element.Add(new XElement("transformationID", Event.BusinessStep));
            if (!string.IsNullOrEmpty(Event.BusinessStep)) element.Add(new XElement("bizStep", Event.BusinessStep));
            if (!string.IsNullOrEmpty(Event.Disposition)) element.Add(new XElement("disposition", Event.Disposition));

            AddReadPoint(Event, element);
            AddBusinessLocation(Event, element);
            AddCustomFields(Event, element, FieldType.EventExtension);

            return new XElement("extension", element);
        }

        private static void AddSourceDest(EpcisEvent Event, XElement element)
        {
            if (Event.SourceDestinationList == null || !Event.SourceDestinationList.Any()) return;

            var source = new XElement("sourceList");
            var destination = new XElement("destinationList");

            foreach (var sourceDest in Event.SourceDestinationList)
            {
                if (sourceDest.Direction == SourceDestinationType.Source)
                    source.Add(new XElement("source", new XAttribute("type", sourceDest.Type), sourceDest.Id));
                else if (sourceDest.Direction == SourceDestinationType.Destination)
                    destination.Add(new XElement("destination", new XAttribute("type", sourceDest.Type), sourceDest.Id));
            }

            if (source.HasElements) AddInExtension(element, source);
            if (destination.HasElements) AddInExtension(element, destination);
        }

        private static void AddBusinessTransactions(EpcisEvent Event, XContainer element)
        {
            if (Event.BusinessTransactions == null || !Event.BusinessTransactions.Any()) return;

            var transactions = new XElement("bizTransactionList");

            foreach (var trans in Event.BusinessTransactions)
                transactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));

            element.Add(transactions);
        }

        private static void AddIlmd(EpcisEvent Event, XContainer element)
        {
            var ilmdElement = new XElement("ilmd");

            AddCustomFields(Event, ilmdElement, FieldType.Ilmd);

            if (ilmdElement.HasAttributes || ilmdElement.HasElements) AddInExtension(element, ilmdElement);
        }

        private static void AddCustomFields(EpcisEvent Event, XContainer element, FieldType type)
        {
            foreach (var rootField in Event.CustomFields.Where(x => x.Type == type && x.ParentId == null))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                AddInnerCustomFields(Event, xmlElement, type, rootField.Id);
                foreach (var attribute in Event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == rootField.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void AddInnerCustomFields(EpcisEvent Event, XContainer element, FieldType type, int parentId)
        {
            foreach (var field in Event.CustomFields.Where(x => x.Type == type && x.ParentId == parentId))
            {
                var xmlElement = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);

                AddInnerCustomFields(Event, xmlElement, type, field.Id);
                foreach (var attribute in Event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == field.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void AddEpcList(EpcisEvent Event, XContainer element)
        {
            var epcList = new XElement("epcList");
            var epcQuantity = new XElement("epcQuantity");
            foreach (var epc in Event.Epcs.Where(x => x.Type == EpcType.List)) epcList.Add(new XElement("epc", epc.Id));
            foreach (var epc in Event.Epcs.Where(x => x.Type == EpcType.Quantity))
            {
                var qtyElement = new XElement("quantityElement");
                qtyElement.Add(new XElement("epcClass", epc.Id));
                if (epc.Quantity.HasValue) qtyElement.Add(new XElement("quantity", epc.Quantity));
                if (!string.IsNullOrEmpty(epc.UnitOfMeasure)) qtyElement.Add(new XElement("uom", epc.UnitOfMeasure));

                epcQuantity.Add(qtyElement);
            }

            if (epcList.HasElements) element.Add(epcList);
            if (epcQuantity.HasElements) AddInExtension(element, epcQuantity);
        }

        private static void AddParentId(EpcisEvent @event, XElement element)
        {
            var parentId = @event.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null) element.Add(new XElement("parentID", parentId.Id));
        }

        private static void AddChildEpcs(EpcisEvent @event, XElement element)
        {
            var childEpcs = @event.Epcs.Where(e => e.Type == EpcType.ChildEpc);
            if (childEpcs.Any()) element.Add(new XElement("childEPCs", childEpcs.Select(x => new XElement("epc", x.Id))));
        }

        private static void AddReadPoint(EpcisEvent Event, XContainer element)
        {
            if (string.IsNullOrEmpty(Event.ReadPoint)) return;

            var readPoint = new XElement("readPoint", new XElement("id", Event.ReadPoint));

            foreach (var ext in Event.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));

            element.Add(readPoint);
        }

        private static void AddBusinessLocation(EpcisEvent Event, XContainer element)
        {
            if (string.IsNullOrEmpty(Event.BusinessLocation)) return;

            var custom = Event.CustomFields.Where(x => x.Type == FieldType.BusinessLocationExtension).Select(field => new XElement(XName.Get(field.Name, field.Namespace), field.TextValue));
            element.Add(new XElement("bizLocation", new XElement("id", Event.BusinessLocation), custom));
        }

        private static void AddInExtension(XContainer container, XElement element)
        {
            var extension = container.Element("extension");
            if (extension == null)
            {
                extension = new XElement("extension");
                container.Add(extension);
            }

            extension.Add(element);
        }

        public XElement Format(MasterData masterData)
        {
            throw new NotImplementedException();
        }
    }
}
