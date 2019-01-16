using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.MasterDatas;
using FasTnT.Model.Responses;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Responses
{
    // TODO: remove duplicate code for all events (times, eventID, ..) (LAA)
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
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

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
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

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
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

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
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

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

        private static XElement FormatTransformationEvent(EpcisEvent @event)
        {
            var element = new XElement("TransformationEvent");

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

            AddEpcList(@event, element);

            if (!string.IsNullOrEmpty(@event.TransformationId)) element.Add(new XElement("transformationID", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.BusinessStep)) element.Add(new XElement("bizStep", @event.BusinessStep));
            if (!string.IsNullOrEmpty(@event.Disposition)) element.Add(new XElement("disposition", @event.Disposition));

            AddReadPoint(@event, element);
            AddBusinessLocation(@event, element);
            AddCustomFields(@event, element, FieldType.EventExtension);

            return new XElement("extension", element);
        }

        private static void AddSourceDest(EpcisEvent @event, XElement element)
        {
            if (@event.SourceDestinationList == null || !@event.SourceDestinationList.Any()) return;

            var source = new XElement("sourceList");
            var destination = new XElement("destinationList");

            foreach (var sourceDest in @event.SourceDestinationList)
            {
                if (sourceDest.Direction == SourceDestinationType.Source)
                    source.Add(new XElement("source", new XAttribute("type", sourceDest.Type), sourceDest.Id));
                else if (sourceDest.Direction == SourceDestinationType.Destination)
                    destination.Add(new XElement("destination", new XAttribute("type", sourceDest.Type), sourceDest.Id));
            }

            if (source.HasElements) AddInExtension(element, source);
            if (destination.HasElements) AddInExtension(element, destination);
        }

        private static void AddBusinessTransactions(EpcisEvent @event, XContainer element)
        {
            if (@event.BusinessTransactions == null || !@event.BusinessTransactions.Any()) return;

            var transactions = new XElement("bizTransactionList");

            foreach (var trans in @event.BusinessTransactions)
                transactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));

            element.Add(transactions);
        }

        private static void AddIlmd(EpcisEvent @event, XContainer element)
        {
            var ilmdElement = new XElement("ilmd");

            AddCustomFields(@event, ilmdElement, FieldType.Ilmd);

            if (ilmdElement.HasAttributes || ilmdElement.HasElements) AddInExtension(element, ilmdElement);
        }

        private static void AddCustomFields(EpcisEvent @event, XContainer element, FieldType type)
        {
            foreach (var rootField in @event.CustomFields.Where(x => x.Type == type && x.ParentId == null))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                AddInnerCustomFields(@event, xmlElement, type, rootField.Id);
                foreach (var attribute in @event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == rootField.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void AddInnerCustomFields(EpcisEvent @event, XContainer element, FieldType type, int parentId)
        {
            foreach (var field in @event.CustomFields.Where(x => x.Type == type && x.ParentId == parentId))
            {
                var xmlElement = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);

                AddInnerCustomFields(@event, xmlElement, type, field.Id);
                foreach (var attribute in @event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == field.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void AddEpcList(EpcisEvent @event, XContainer element)
        {
            var epcList = new XElement("epcList");
            var epcQuantity = new XElement("epcQuantity");
            foreach (var epc in @event.Epcs.Where(x => x.Type == EpcType.List)) epcList.Add(new XElement("epc", epc.Id));
            foreach (var epc in @event.Epcs.Where(x => x.Type == EpcType.Quantity))
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

        private static void AddReadPoint(EpcisEvent @event, XContainer element)
        {
            if (string.IsNullOrEmpty(@event.ReadPoint)) return;

            var readPoint = new XElement("readPoint", new XElement("id", @event.ReadPoint));

            foreach (var ext in @event.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));

            element.Add(readPoint);
        }

        private static void AddBusinessLocation(EpcisEvent @event, XContainer element)
        {
            if (string.IsNullOrEmpty(@event.BusinessLocation)) return;

            var custom = @event.CustomFields.Where(x => x.Type == FieldType.BusinessLocationExtension).Select(field => new XElement(XName.Get(field.Name, field.Namespace), field.TextValue));
            element.Add(new XElement("bizLocation", new XElement("id", @event.BusinessLocation), custom));
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

        // TODO: handle better MasterData...
        public static XElement Format(EpcisMasterData masterData)
        {
            return new XElement("VocabularyElement", new XAttribute("id", masterData.Id));
        }
    }
}
