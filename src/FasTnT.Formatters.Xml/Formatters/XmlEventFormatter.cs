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
    public class XmlEventFormatter
    { 
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

        private Dictionary<EventType, FormatAction[]> eventBuilder;
        private XElement _extensionField;

        public XmlEventFormatter()
        {
            eventBuilder = new Dictionary<EventType, FormatAction[]>
            {
                { EventType.Object, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, ExtensionIlmd, SourceDest, AddExtensionField, AddEventExtension } },
                { EventType.Quantity, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddExtensionField, AddEventExtension } },
                { EventType.Aggregation, new FormatAction[]{ ParentId, ChildEpcs, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddExtensionField, AddEventExtension } },
                { EventType.Transaction, new FormatAction[]{ EpcList, Action, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, AddExtensionField, AddEventExtension } },
                { EventType.Transformation, new FormatAction[]{ EpcList, TransformationId, BizStep, Disposition, ReadPoint, BizLocation, BizTransaction, SourceDest, Ilmd, AddExtensionField, AddEventExtension } },
            };
        }

        public XElement Format(EpcisEvent epcisEvent)
        {
            _extensionField = new XElement("extension");

            var element = CreateEvent(epcisEvent);
            eventBuilder[epcisEvent.Type].ForEach(a => a(epcisEvent, element));
            if (epcisEvent.Type == EventType.Transformation) element = new XElement("extension", element);

            return element;
        }

        private XElement CreateEvent(EpcisEvent @event)
        {
            var element = new XElement(@event.Type.DisplayName);

            element.Add(new XElement("eventTime", @event.EventTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)));
            element.Add(new XElement("recordTime", @event.CaptureTime.ToString(DateTimeFormat)));
            element.Add(new XElement("eventTimeZoneOffset", @event.EventTimeZoneOffset.Representation));
            if (@event.ErrorDeclaration != null) AddErrorDeclaration(@event.ErrorDeclaration, element);
            if (!string.IsNullOrEmpty(@event.EventId)) element.Add(new XElement("eventID", @event.EventId));

            return element;
        }

        private void AddErrorDeclaration(ErrorDeclaration eventError, XElement element)
        {
            var correctiveEventIds = eventError.CorrectiveEventIds.Any() ? new XElement("correctiveEventIDs", eventError.CorrectiveEventIds.Select(x => new XElement("correctiveEventId", x.CorrectiveId))) : null;
            var errorDeclaration = new XElement("errorDeclaration", new XElement("declarationTime", eventError.DeclarationTime), new XElement("reason", eventError.Reason), correctiveEventIds);
            element.Add(new XElement("baseExtension", errorDeclaration));
        }

        public void EpcList(EpcisEvent evt, XContainer element)
        {
            var epcList = new XElement("epcList", evt.Epcs.Where(x => x.Type == EpcType.List).Select(e => new XElement("epc", e.Id)));
            var inputEpcList = new XElement("inputEPCList", evt.Epcs.Where(x => x.Type == EpcType.InputEpc).Select(e => new XElement("epc", e.Id)));
            var inputQuantity = new XElement("inputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.InputQuantity).Select(FormatQuantity));
            var quantityList = new XElement("quantityList", evt.Epcs.Where(x => x.Type == EpcType.Quantity).Select(FormatQuantity));
            var outputQuantity = new XElement("outputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.OutputQuantity).Select(FormatQuantity));
            var outputEpcList = new XElement("outputEPCList", evt.Epcs.Where(x => x.Type == EpcType.OutputEpc).Select(e => new XElement("epc", e.Id)));

            if (epcList.HasElements) element.Add(epcList);
            if (inputEpcList.HasElements) element.Add(inputEpcList);
            if (inputQuantity.HasElements) element.Add(inputQuantity);
            if (quantityList.HasElements) AddInExtension(element, quantityList);
            if (outputQuantity.HasElements) element.Add(outputQuantity);
            if (outputEpcList.HasElements) element.Add(outputEpcList);
        }

        public XElement FormatQuantity(Epc epc)
        {
            var qtyElement = new XElement("quantityElement");
            qtyElement.Add(new XElement("epcClass", epc.Id));
            if (epc.Quantity.HasValue) qtyElement.Add(new XElement("quantity", epc.Quantity));
            if (!string.IsNullOrEmpty(epc.UnitOfMeasure)) qtyElement.Add(new XElement("uom", epc.UnitOfMeasure));

            return qtyElement;
        }

        public void Action(EpcisEvent evt, XContainer container)
        {
            container.Add(new XElement("action", evt.Action.ToString().ToUpper(CultureInfo.InvariantCulture)));
        }

        public void BizStep(EpcisEvent evt, XContainer container)
        {
            if(!string.IsNullOrEmpty(evt.BusinessStep)) container.Add(new XElement("bizStep", evt.BusinessStep));
        }

        public void Disposition(EpcisEvent evt, XContainer container)
        {
            if (!string.IsNullOrEmpty(evt.Disposition)) container.Add(new XElement("disposition", evt.Disposition));
        }

        public void TransformationId(EpcisEvent evt, XContainer container)
        {
            if (!string.IsNullOrEmpty(evt.TransformationId)) container.Add(new XElement("transformationID", evt.TransformationId));
        }

        private void SourceDest(EpcisEvent @event, XContainer element)
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

        private void BizTransaction(EpcisEvent @event, XContainer element)
        {
            if (@event.BusinessTransactions == null || !@event.BusinessTransactions.Any()) return;

            var transactions = new XElement("bizTransactionList");

            foreach (var trans in @event.BusinessTransactions)
                transactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));

            element.Add(transactions);
        }

        private void ExtensionIlmd(EpcisEvent @event, XContainer element) => Ilmd(@event, element, true);
        private void Ilmd(EpcisEvent @event, XContainer element) => Ilmd(@event, element, false);

        private void Ilmd(EpcisEvent @event, XContainer element, bool inExtension)
        {
            var ilmdElement = new XElement("ilmd");
            CustomFields(@event, ilmdElement, FieldType.Ilmd);

            if (ilmdElement.HasAttributes || ilmdElement.HasElements)
            {
                if (inExtension) AddInExtension(element, ilmdElement);
                else element.Add(ilmdElement);
            }
        }

        public void AddExtensionField(EpcisEvent @event, XContainer element)
        {
            if (_extensionField.HasElements || _extensionField.HasAttributes)
            {
                element.Add(_extensionField);
            }
        }

        public void AddEventExtension(EpcisEvent @event, XContainer element)
        {
            CustomFields(@event, element, FieldType.EventExtension);
        }

        private void CustomFields(EpcisEvent @event, XContainer element, FieldType type)
        {
            foreach (var rootField in @event.CustomFields.Where(x => x.Type == type))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                InnerCustomFields(xmlElement, type, rootField);
                foreach (var attribute in rootField.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }

        private void InnerCustomFields(XContainer element, FieldType type, CustomField parent)
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

        private void ParentId(EpcisEvent @event, XContainer element)
        {
            var parentId = @event.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null) element.Add(new XElement("parentID", parentId.Id));
        }

        private void ChildEpcs(EpcisEvent @event, XContainer element)
        {
            var childEpcs = @event.Epcs.Where(e => e.Type == EpcType.ChildEpc).Select(x => new XElement("epc", x.Id));
            var childQuantity = new XElement("childQuantityList", @event.Epcs.Where(x => x.Type == EpcType.ChildQuantity).Select(FormatQuantity));

            element.Add(new XElement("childEPCs", childEpcs));
            if (childQuantity.HasElements) AddInExtension(element, childQuantity);
        }

        private void ReadPoint(EpcisEvent @event, XContainer element)
        {
            if (string.IsNullOrEmpty(@event.ReadPoint)) return;

            var readPoint = new XElement("readPoint", new XElement("id", @event.ReadPoint));

            foreach (var ext in @event.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));

            element.Add(readPoint);
        }

        private void BizLocation(EpcisEvent @event, XContainer element)
        {
            if (string.IsNullOrEmpty(@event.BusinessLocation)) return;

            var custom = @event.CustomFields.Where(x => x.Type == FieldType.BusinessLocationExtension).Select(field => new XElement(XName.Get(field.Name, field.Namespace), field.TextValue));
            element.Add(new XElement("bizLocation", new XElement("id", @event.BusinessLocation), custom));
        }

        private void AddInExtension(XContainer container, XElement element)
        {
            _extensionField.Add(element);
        }
    }
}
