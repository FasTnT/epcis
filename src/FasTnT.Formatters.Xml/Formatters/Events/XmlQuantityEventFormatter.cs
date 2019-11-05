using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlQuantityEventFormatter
    {
        private XElement _root;
        private XElement _extension = new XElement("extension");

        public XElement Process(EpcisEvent quantityEvent)
        {
            _root = XmlEventFormatter.CreateEvent("QuantityEvent", quantityEvent);

            AddQuantityEpc(quantityEvent);
            AddBusinessStep(quantityEvent);
            AddDisposition(quantityEvent);
            AddReadPoint(quantityEvent);
            AddBusinessLocation(quantityEvent);
            AddBusinessTransactions(quantityEvent);
            AddSourceDestinations(quantityEvent);
            AddEventExtension(quantityEvent);
            AddExtensionField();
            AddCustomFields(quantityEvent);

            return _root;
        }

        private void AddCustomFields(EpcisEvent evt)
        {
            _root.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.CustomField));
        }

        private void AddExtensionField()
        {
            if (_extension.HasElements || _extension.HasAttributes)
            {
                _root.Add(_extension);
            }
        }

        private void AddQuantityEpc(EpcisEvent evt)
        {
            var epc = evt.Epcs.Single();

            _root.Add(new XElement("epcClass", epc.Id), new XElement("quantity", epc.Quantity));
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
            _extension.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.EventExtension));
        }
    }
}
