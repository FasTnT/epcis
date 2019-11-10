using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlAggregationEventFormatter : BaseV1_2EventFormatter
    {
        public XElement Process(EpcisEvent aggregationEvent)
        {
            Root = XmlEventFormatter.CreateEvent("AggregationEvent", aggregationEvent);

            AddParentId(aggregationEvent);
            AddChildEpcs(aggregationEvent);
            AddAction(aggregationEvent);
            AddBusinessStep(aggregationEvent);
            AddDisposition(aggregationEvent);
            AddReadPoint(aggregationEvent);
            AddBusinessLocation(aggregationEvent);
            AddBusinessTransactions(aggregationEvent);
            AddSourceDestinations(aggregationEvent, Extension);
            AddEventExtension(aggregationEvent);
            AddExtensionField();
            AddCustomFields(aggregationEvent);

            return Root;
        }

        private void AddParentId(EpcisEvent evt)
        {
            var parentId = evt.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null)
            {
                Root.Add(new XElement("parentID", parentId.Id));
            }
        }

        private void AddChildEpcs(EpcisEvent evt)
        {
            var childQuantity = new XElement("childQuantityList", evt.Epcs.Where(x => x.Type == EpcType.ChildQuantity).Select(XmlEventFormatter.FormatQuantity));

            Root.Add(new XElement("childEPCs", evt.Epcs.Where(e => e.Type == EpcType.ChildEpc).Select(x => new XElement("epc", x.Id))));
            if (childQuantity.HasElements) Extension.Add(childQuantity);
        }
    }
}
