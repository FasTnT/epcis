using FasTnT.Model;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlQuantityEventFormatter : BaseV1EventFormatter
    {
        public XElement Process(EpcisEvent quantityEvent)
        {
            Root = XmlEventFormatter.CreateEvent("QuantityEvent", quantityEvent);

            AddQuantityEpc(quantityEvent);
            AddBusinessStep(quantityEvent);
            AddDisposition(quantityEvent);
            AddReadPoint(quantityEvent);
            AddBusinessLocation(quantityEvent);
            AddBusinessTransactions(quantityEvent);
            AddEventExtension(quantityEvent);
            AddExtensionField();
            AddCustomFields(quantityEvent);

            return Root;
        }

        private void AddQuantityEpc(EpcisEvent evt)
        {
            var epc = evt.Epcs.Single();

            Root.Add(new XElement("epcClass", epc.Id), new XElement("quantity", epc.Quantity));
        }
    }
}
