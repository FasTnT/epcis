using FasTnT.Model;
using FasTnT.Parsers.Xml.Formatters.Implementation;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{

    public class XmlTransactionEventFormatter : BaseV1EventFormatter
    {
        public XElement Process(EpcisEvent epcisEvent)
        {
            Root = XmlEventFormatter.CreateEvent("TransactionEvent", epcisEvent);

            AddBusinessTransactions(epcisEvent);
            AddEpcList(epcisEvent);
            AddAction(epcisEvent);
            AddBusinessStep(epcisEvent);
            AddDisposition(epcisEvent);
            AddReadPoint(epcisEvent);
            AddBusinessLocation(epcisEvent);
            AddEventExtension(epcisEvent);
            AddSourceDestinations(epcisEvent, Extension);
            AddExtensionField();
            AddCustomFields(epcisEvent);

            return Root;
        }
    }
}