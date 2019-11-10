using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlObjectEventFormatter : BaseV1EventFormatter
    {
        public XElement Process(EpcisEvent objectEvent)
        {
            Root = XmlEventFormatter.CreateEvent("ObjectEvent", objectEvent);

            AddEpcList(objectEvent);
            AddAction(objectEvent);
            AddBusinessStep(objectEvent);
            AddDisposition(objectEvent);
            AddReadPoint(objectEvent);
            AddBusinessLocation(objectEvent);
            AddBusinessTransactions(objectEvent);
            AddEventExtension(objectEvent);
            AddSourceDestinations(objectEvent, Extension);
            AddIlmdFields(objectEvent, Extension);
            AddExtensionField();
            AddCustomFields(objectEvent);

            return Root;
        }
    }
}
