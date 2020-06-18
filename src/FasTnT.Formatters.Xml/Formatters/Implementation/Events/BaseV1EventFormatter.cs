using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using FasTnT.Parsers.Xml.Utils;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public abstract class BaseV1EventFormatter
    {
        protected XElement Root;
        protected XElement Extension = new XElement("extension");

        protected virtual void AddCustomFields(EpcisEvent evt)
        {
            Root.AddIfAny(XmlEventFormatter.GenerateCustomFields(evt, FieldType.CustomField));
        }

        protected virtual void AddExtensionField()
        {
            Root.AddIfNotNull(Extension);
        }

        protected virtual void AddEpcList(EpcisEvent evt)
        {
            var inputEpcList = new XElement("inputEPCList", XmlEventFormatter.FormatEpcList(evt, EpcType.InputEpc));
            var quantityList = new XElement("quantityList", XmlEventFormatter.FormatEpcQuantity(evt, EpcType.Quantity));

            Root.Add(new XElement("epcList", XmlEventFormatter.FormatEpcList(evt, EpcType.List)));
            if (inputEpcList.HasElements) Root.Add(inputEpcList);
            if (quantityList.HasElements) Extension.Add(quantityList);
        }

        protected virtual void AddAction(EpcisEvent evt)
        {
            Root.Add(new XElement("action", evt.Action.DisplayName));
        }

        protected virtual void AddBusinessStep(EpcisEvent evt)
        {
            Root.AddIfNotNull(XmlEventFormatter.GenerateBusinesStep(evt));
        }

        protected virtual void AddDisposition(EpcisEvent evt)
        {
            Root.AddIfNotNull(XmlEventFormatter.GenerateDisposition(evt));
        }

        protected virtual void AddReadPoint(EpcisEvent evt)
        {
            Root.AddIfNotNull(XmlEventFormatter.GenerateReadPoint(evt));
        }

        protected virtual void AddBusinessLocation(EpcisEvent evt)
        {
            Root.AddIfNotNull(XmlEventFormatter.GenerateBusinessLocation(evt));
        }

        protected virtual void AddBusinessTransactions(EpcisEvent evt)
        {
            Root.AddIfNotNull(XmlEventFormatter.GenerateBusinessTransactions(evt));
        }

        protected virtual void AddIlmdFields(EpcisEvent evt, XElement element)
        {
            var ilmdElement = new XElement("ilmd", XmlEventFormatter.GenerateCustomFields(evt, FieldType.Ilmd));
            element.AddIfNotNull(ilmdElement);
        }

        protected virtual void AddSourceDestinations(EpcisEvent evt, XElement element)
        {
            element.AddIfAny(XmlEventFormatter.GenerateSourceDest(evt));
        }

        protected virtual void AddEventExtension(EpcisEvent evt)
        {
            var extension = new XElement("extension", XmlEventFormatter.GenerateCustomFields(evt, FieldType.Extension));
            Extension.AddIfNotNull(extension);
        }
    }
}