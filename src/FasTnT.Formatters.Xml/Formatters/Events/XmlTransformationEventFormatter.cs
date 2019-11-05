using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlTransformationEventFormatter
    {
        private XElement _root;
        private XElement _extension = new XElement("extension");

        public XElement Process(EpcisEvent transformationEvent)
        {
            _root = XmlEventFormatter.CreateEvent("TransformationEvent", transformationEvent);

            AddEpcList(transformationEvent);
            AddBusinessStep(transformationEvent);
            AddDisposition(transformationEvent);
            AddReadPoint(transformationEvent);
            AddBusinessLocation(transformationEvent);
            AddBusinessTransactions(transformationEvent);
            AddSourceDestinations(transformationEvent);
            AddEventExtension(transformationEvent);
            AddExtensionField();
            AddCustomFields(transformationEvent);

            return new XElement("extension", _root);
        }

        private void AddCustomFields(EpcisEvent evt)
        {
            _root.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.CustomField));
        }

        private void AddExtensionField()
        {
            _root.AddIfNotNull(_extension);
        }

        private void AddEpcList(EpcisEvent evt)
        {
            var inputEpcList = new XElement("inputEPCList", evt.Epcs.Where(x => x.Type == EpcType.InputEpc).Select(e => new XElement("epc", e.Id)));
            var inputQuantity = new XElement("inputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.InputQuantity).Select(XmlEventFormatter.FormatQuantity));
            var outputQuantity = new XElement("outputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.OutputQuantity).Select(XmlEventFormatter.FormatQuantity));
            var outputEpcList = new XElement("outputEPCList", evt.Epcs.Where(x => x.Type == EpcType.OutputEpc).Select(e => new XElement("epc", e.Id)));

            if (inputEpcList.HasElements) _root.Add(inputEpcList);
            if (inputQuantity.HasElements) _root.Add(inputQuantity);
            if (outputEpcList.HasElements) _root.Add(outputEpcList);
            if (outputQuantity.HasElements) _root.Add(outputQuantity);
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
            _root.AddIfAny(XmlEventFormatter.GenerateSourceDest(evt));
        }

        public void AddEventExtension(EpcisEvent evt)
        {
            _extension.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.EventExtension));
        }
    }
}