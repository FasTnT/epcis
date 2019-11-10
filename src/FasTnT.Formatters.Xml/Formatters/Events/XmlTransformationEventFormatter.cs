using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
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
            var inputEpcList = new XElement("inputEPCList", evt.Epcs.Where(x => x.Type == EpcType.InputEpc).Select(e => new XElement("epc", e.Id)));
            var inputQuantity = new XElement("inputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.InputQuantity).Select(XmlEventFormatter.FormatQuantity));
            var outputQuantity = new XElement("outputQuantityList", evt.Epcs.Where(x => x.Type == EpcType.OutputQuantity).Select(XmlEventFormatter.FormatQuantity));
            var outputEpcList = new XElement("outputEPCList", evt.Epcs.Where(x => x.Type == EpcType.OutputEpc).Select(e => new XElement("epc", e.Id)));

            if (inputEpcList.HasElements) Root.Add(inputEpcList);
            if (inputQuantity.HasElements) Root.Add(inputQuantity);
            if (outputEpcList.HasElements) Root.Add(outputEpcList);
            if (outputQuantity.HasElements) Root.Add(outputQuantity);
        }
    }
}