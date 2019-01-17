using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using MoreLinq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using FormatAction = System.Action<FasTnT.Model.EpcisEvent, System.Xml.Linq.XContainer>;

namespace FasTnT.Formatters.Xml.Responses
{
    public static class XmlEventFormatter
    { 
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        static Dictionary<EventType, FormatAction[]> eventBuilder = new Dictionary<EventType, FormatAction[]>
        {
            { EventType.Object, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, Ilmd, SourceDest, AddEventExtension } },
            { EventType.Quantity, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddEventExtension } },
            { EventType.Aggregation, new FormatAction[]{ ParentId, ChildEpcs, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddEventExtension } },
            { EventType.Transaction, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddEventExtension } },
            { EventType.Transformation, new FormatAction[]{ EpcList, TransformationId, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddEventExtension } },
        };

        public static XElement Format(EpcisEvent epcisEvent)
        {
            var element = CreateEvent(epcisEvent);
            eventBuilder[epcisEvent.Type].ForEach(a => a(epcisEvent, element));

            return element;
        }

        private static XElement CreateEvent(EpcisEvent @event)
        {
            var element = new XElement(@event.Type.DisplayName);

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

            return element;
        }

        public static void EpcList(EpcisEvent evt, XContainer element)
        {
            var epcList = new XElement("epcList");
            var epcQuantity = new XElement("epcQuantity");
            foreach (var epc in evt.Epcs.Where(x => x.Type == EpcType.List)) epcList.Add(new XElement("epc", epc.Id));
            foreach (var epc in evt.Epcs.Where(x => x.Type == EpcType.Quantity))
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

        public static void Action(EpcisEvent evt, XContainer container)
        {
            container.Add(new XElement("action", evt.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));
        }

        public static void BizStep(EpcisEvent evt, XContainer container)
        {
            if(!string.IsNullOrEmpty(evt.BusinessStep)) container.Add(new XElement("bizStep", evt.BusinessStep));
        }

        public static void Disposition(EpcisEvent evt, XContainer container)
        {
            if (!string.IsNullOrEmpty(evt.Disposition)) container.Add(new XElement("disposition", evt.Disposition));
        }

        public static void TransformationId(EpcisEvent evt, XContainer container)
        {
            if (!string.IsNullOrEmpty(evt.TransformationId)) container.Add(new XElement("transformationID", evt.TransformationId));
        }

        private static void SourceDest(EpcisEvent @event, XContainer element)
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

        private static void BizTransaction(EpcisEvent @event, XContainer element)
        {
            if (@event.BusinessTransactions == null || !@event.BusinessTransactions.Any()) return;

            var transactions = new XElement("bizTransactionList");

            foreach (var trans in @event.BusinessTransactions)
                transactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));

            element.Add(transactions);
        }

        private static void Ilmd(EpcisEvent @event, XContainer element)
        {
            var ilmdElement = new XElement("ilmd");

            CustomFields(@event, ilmdElement, FieldType.Ilmd);

            if (ilmdElement.HasAttributes || ilmdElement.HasElements) AddInExtension(element, ilmdElement);
        }

        public static void AddEventExtension(EpcisEvent @event, XContainer element)
        {
            CustomFields(@event, element, FieldType.EventExtension);
        }

        private static void CustomFields(EpcisEvent @event, XContainer element, FieldType type)
        {
            foreach (var rootField in @event.CustomFields.Where(x => x.Type == type && x.ParentId == null))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                InnerCustomFields(@event, xmlElement, type, rootField.Id);
                foreach (var attribute in @event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == rootField.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void InnerCustomFields(EpcisEvent @event, XContainer element, FieldType type, int parentId)
        {
            foreach (var field in @event.CustomFields.Where(x => x.Type == type && x.ParentId == parentId))
            {
                var xmlElement = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);

                InnerCustomFields(@event, xmlElement, type, field.Id);
                foreach (var attribute in @event.CustomFields.Where(x => x.Type == FieldType.Attribute && x.ParentId == field.Id))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private static void ParentId(EpcisEvent @event, XContainer element)
        {
            var parentId = @event.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null) element.Add(new XElement("parentID", parentId.Id));
        }

        private static void ChildEpcs(EpcisEvent @event, XContainer element)
        {
            var childEpcs = @event.Epcs.Where(e => e.Type == EpcType.ChildEpc);
            if (childEpcs.Any()) element.Add(new XElement("childEPCs", childEpcs.Select(x => new XElement("epc", x.Id))));
        }

        private static void ReadPoint(EpcisEvent @event, XContainer element)
        {
            if (string.IsNullOrEmpty(@event.ReadPoint)) return;

            var readPoint = new XElement("readPoint", new XElement("id", @event.ReadPoint));

            foreach (var ext in @event.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));

            element.Add(readPoint);
        }

        private static void BizLocation(EpcisEvent @event, XContainer element)
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
    }
}
