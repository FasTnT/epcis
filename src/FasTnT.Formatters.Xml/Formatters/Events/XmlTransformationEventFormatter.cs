using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlTransformationEventFormatter : BaseV1_2EventFormatter
    {
        public XElement Process(EpcisEvent transformationEvent)
        {
            Root = XmlEventFormatter.CreateEvent("TransformationEvent", transformationEvent);

            AddInputOutputEpcList(transformationEvent);
            AddBusinessStep(transformationEvent);
            AddDisposition(transformationEvent);
            AddReadPoint(transformationEvent);
            AddBusinessLocation(transformationEvent);
            AddBusinessTransactions(transformationEvent);
            AddSourceDestinations(transformationEvent, Root);
            AddEventExtension(transformationEvent);
            AddIlmdFields(transformationEvent, Root);
            AddExtensionField();
            AddCustomFields(transformationEvent);

            return new XElement("extension", Root);
        }

        private void AddInputOutputEpcList(EpcisEvent evt)
        {
            var inputEpcList = new XElement("inputEPCList", XmlEventFormatter.FormatEpcList(evt, EpcType.InputEpc));
            var inputQuantity = new XElement("inputQuantityList", XmlEventFormatter.FormatEpcQuantity(evt, EpcType.InputQuantity));
            var outputQuantity = new XElement("outputQuantityList", XmlEventFormatter.FormatEpcQuantity(evt, EpcType.OutputQuantity));
            var outputEpcList = new XElement("outputEPCList", XmlEventFormatter.FormatEpcList(evt, EpcType.OutputEpc));

            if (inputEpcList.HasElements) Root.Add(inputEpcList);
            if (inputQuantity.HasElements) Root.Add(inputQuantity);
            if (outputEpcList.HasElements) Root.Add(outputEpcList);
            if (outputQuantity.HasElements) Root.Add(outputQuantity);
        }
    }
}