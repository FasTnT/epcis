using FasTnT.Domain;
using FasTnT.Formatters.Xml.Requests;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml
{
    public static class XElementExtensions
    {
        public static EventType ToEventType(this XElement element) => Enumeration.GetByDisplayName<EventType>(element.Name.LocalName);
        public static EventAction ToEventAction(this XElement element) => Enumeration.GetByDisplayName<EventAction>(element.Value);

        public static void ParseEpcListInto(this XElement element, EpcisEvent destination)
        {
            foreach (var epc in element.Elements()) destination.Epcs.Add(new Epc { Type = EpcType.List, Id = epc.Value });
        }

        public static void ParseEpcListInto(this XElement element, EpcisEvent destination, bool isInput)
        {
            var type = isInput ? EpcType.InputEpc : EpcType.OutputEpc;

            foreach (var epc in element.Elements("epc")) destination.Epcs.Add(new Epc { Type = type, Id = epc.Value });
        }

        public static void ParseQuantityListInto(this XElement element, EpcisEvent destination, bool isInput)
        {
            foreach (var epc in element.Elements("quantityElement"))
            {
                destination.Epcs.Add(new Epc
                {
                    Type = isInput ? EpcType.InputQuantity : EpcType.OutputQuantity,
                    Id = epc.Element("epcClass").Value,
                    IsQuantity = true,
                    Quantity = float.Parse(element.Element("Quantity").Value, CultureInfo.InvariantCulture),
                    UnitOfMeasure = element.Element("uom") != null ? element.Element("uom").Value : null
                });
            }
        }

        public static void ParseChildEpcListInto(this XElement element, EpcisEvent destination)
        {
            foreach (var epc in element.Elements()) destination.Epcs.Add(new Epc { Type = EpcType.ChildEpc, Id = epc.Value });
        }

        public static IList<BusinessTransaction> ToBusinessTransactions(this XElement element)
        {
            return element.Elements("bizTransaction").Select(child => new BusinessTransaction { Type = child.Attribute("type").Value, Id = child.Value }).ToList();
        }

        public static void ParseReadPoint(this XElement element, EpcisEvent Event)
        {
            Event.ReadPoint = element.Element("id").Value;

            foreach (var innerElement in element.Elements().Where(x => x.Name.Namespace != XNamespace.None))
            {
                Event.CustomFields.Add(XmlEventsParser.ParseCustomField(innerElement, Event, FieldType.ReadPointExtension));
            }
        }

        public static void ParseSourceInto(this XElement element, IList<SourceDestination> list)
        {
            foreach (var child in element.Elements("source"))
            {
                list.Add(new SourceDestination
                {
                    Type = child.Attribute("type").Value,
                    Id = child.Value,
                    Direction = SourceDestinationType.Source
                });
            }
        }

        public static void ParseDestinationInto(this XElement element, IList<SourceDestination> list)
        {
            foreach (var child in element.Elements("destination"))
            {
                list.Add(new SourceDestination
                {
                    Type = child.Attribute("type").Value,
                    Id = child.Value,
                    Direction = SourceDestinationType.Destination
                });
            }
        }

        public static void ParseBusinessLocation(this XElement element, EpcisEvent Event)
        {
            foreach (var innerElement in element.Elements().Where(x => !new[] { "id", "corrective" }.Contains(x.Name.LocalName)))
            {
                Event.CustomFields.Add(XmlEventsParser.ParseCustomField(innerElement, Event, FieldType.BusinessLocationExtension));
            }

            Event.BusinessLocation = element.Element("id").Value;
        }

        public static ErrorDeclaration ToErrorDeclaration(this XElement element, EpcisEvent Event)
        {
            foreach (var innerElement in element.Elements().Where(x => !new[] { "id", "corrective" }.Contains(x.Name.LocalName)))
            {
                Event.CustomFields.Add(XmlEventsParser.ParseCustomField(innerElement, Event, FieldType.ErrorDeclarationExtension));
            }

            var declarationTime = DateTime.Parse(element.Element("declarationTime").Value, CultureInfo.InvariantCulture);
            return new ErrorDeclaration { DeclarationTime = declarationTime, Reason = element.Element("reason").Value };
        }
    }
}
