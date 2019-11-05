using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlAggregationEventFormatter
    {
        private XElement _root;
        private XElement _extension = new XElement("extension");

        public XElement Process(EpcisEvent aggregationEvent)
        {
            _root = XmlEventFormatter.CreateEvent("AggregationEvent", aggregationEvent);

            AddParentId(aggregationEvent);
            AddChildEpcs(aggregationEvent);
            AddAction(aggregationEvent);
            AddBusinessStep(aggregationEvent);
            AddDisposition(aggregationEvent);
            AddReadPoint(aggregationEvent);
            AddBusinessLocation(aggregationEvent);
            AddBusinessTransactions(aggregationEvent);
            AddSourceDestinations(aggregationEvent);
            AddEventExtension(aggregationEvent);
            AddExtensionField();
            AddCustomFields(aggregationEvent);

            return _root;
        }

        private void AddParentId(EpcisEvent evt)
        {
            var parentId = evt.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null)
            {
                _root.Add(new XElement("parentID", parentId.Id));
            }
        }

        private void AddCustomFields(EpcisEvent evt)
        {
            _root.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.CustomField));
        }

        private void AddExtensionField()
        {
            _root.AddIfNotNull(_extension);
        }

        private void AddChildEpcs(EpcisEvent evt)
        {
            var childQuantity = new XElement("childQuantityList", evt.Epcs.Where(x => x.Type == EpcType.ChildQuantity).Select(XmlEventFormatter.FormatQuantity));

            _root.Add(new XElement("childEPCs", evt.Epcs.Where(e => e.Type == EpcType.ChildEpc).Select(x => new XElement("epc", x.Id))));
            if (childQuantity.HasElements) _extension.Add(childQuantity);
        }

        private void AddAction(EpcisEvent objectEvent)
        {
            _root.Add(new XElement("action", objectEvent.Action.DisplayName));
        }

        private void AddBusinessStep(EpcisEvent evt)
        {
            _root.AddIfNotNull(XmlEventFormatter.GenerateBusinesStep(evt));
        }

        public void AddDisposition(EpcisEvent evt)
        {
            _root.AddIfNotNull(XmlEventFormatter.GenerateDisposition(evt));
        }
        private void AddReadPoint(EpcisEvent evt)
        {
            _root.AddIfNotNull(XmlEventFormatter.GenerateReadPoint(evt));
        }

        private void AddBusinessLocation(EpcisEvent evt)
        {
            _root.AddIfNotNull(XmlEventFormatter.GenerateBusinessLocation(evt));
        }

        private void AddBusinessTransactions(EpcisEvent evt)
        {
            _root.AddIfNotNull(XmlEventFormatter.GenerateBusinessTransactions(evt));
        }

        private void AddIlmdFields(EpcisEvent evt)
        {
            _extension.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.Ilmd));
        }

        private void AddSourceDestinations(EpcisEvent evt)
        {
            _extension.AddIfAny(XmlEventFormatter.GenerateSourceDest(evt));
        }

        public void AddEventExtension(EpcisEvent evt)
        {
            var extension = new XElement("extension", XmlEventFormatter.GenerateCustomFields(evt, FieldType.EventExtension));
            _extension.AddIfNotNull(extension);
        }
    }
}
