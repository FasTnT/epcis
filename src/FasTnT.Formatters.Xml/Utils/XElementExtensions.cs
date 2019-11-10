using FasTnT.Formatters.Xml.Parsers;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml
{
    public static class XElementExtensions
    {
        public static void ParseEpcListInto(this XElement element, EpcType type, EpcisEvent destination)
        {
            foreach (var epc in element.Elements("epc")) destination.Epcs.Add(new Epc { Type = type, Id = epc.Value });
        }

        public static void ParseQuantityListInto(this XElement element, EpcisEvent destination, EpcType type)
        {
            foreach (var epc in element.Elements("quantityElement"))
            {
                destination.Epcs.Add(new Epc
                {
                    Type = type,
                    Id = epc.Element("epcClass").Value,
                    IsQuantity = true,
                    Quantity = float.Parse(epc.Element("quantity").Value, CultureInfo.InvariantCulture),
                    UnitOfMeasure = epc.Element("uom")?.Value
                });
            }
        }

        public static void ParseChildEpcListInto(this XElement element, EpcisEvent destination)
        {
            foreach (var epc in element.Elements()) destination.Epcs.Add(new Epc { Type = EpcType.ChildEpc, Id = epc.Value });
        }

        public static List<BusinessTransaction> ToBusinessTransactions(this XElement element)
        {
            return element.Elements("bizTransaction").Select(child => new BusinessTransaction { Type = child.Attribute("type").Value, Id = child.Value }).ToList();
        }

        public static void ParseReadPoint(this XElement element, EpcisEvent Event)
        {
            Event.ReadPoint = element.Element("id").Value;

            foreach (var innerElement in element.Elements().Where(x => x.Name.Namespace != XNamespace.None))
            {
                Event.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(innerElement, FieldType.ReadPointExtension));
            }
        }

        public static void ParseSourceDest(this XElement element, SourceDestinationType type, IList<SourceDestination> list)
        {
            foreach (var child in element.Elements(type.DisplayName))
            {
                list.Add(new SourceDestination
                {
                    Type = child.Attribute("type").Value,
                    Id = child.Value,
                    Direction = type
                });
            }
        }

        public static void ParseBusinessLocation(this XElement element, EpcisEvent Event)
        {
            foreach (var innerElement in element.Elements().Where(x => !new[] { "id", "corrective" }.Contains(x.Name.LocalName)))
            {
                Event.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(innerElement, FieldType.BusinessLocationExtension));
            }

            Event.BusinessLocation = element.Element("id").Value;
        }

        public static ErrorDeclaration ToErrorDeclaration(this XElement element, EpcisEvent Event)
        {
            foreach (var innerElement in element.Elements().Where(x => !new[] { "id", "corrective", "declarationTime", "reason", "correctiveEventIDs" }.Contains(x.Name.LocalName)))
            {
                Event.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(innerElement, FieldType.ErrorDeclarationExtension));
            }

            var declarationTime = DateTime.Parse(element.Element("declarationTime").Value, CultureInfo.InvariantCulture);
            var correctiveEventIds = new List<CorrectiveEventId>();
            ParseCorrectiveEventIds(element, correctiveEventIds);

            return new ErrorDeclaration { DeclarationTime = declarationTime, Reason = element.Element("reason").Value, CorrectiveEventIds = correctiveEventIds.ToArray() };
        }
        
        public static void ParseCorrectiveEventIds(this XElement element, IList<CorrectiveEventId> list)
        {
            var correctiveEventIdList = element.Element("correctiveEventIDs");

            if (correctiveEventIdList != null)
            {
                foreach (var child in correctiveEventIdList.Elements("correctiveEventID"))
                {
                    list.Add(new CorrectiveEventId
                    {
                        CorrectiveId = child.Value
                    });
                }
            }
        }

        public static void AddIfAny(this XElement root, IEnumerable<XElement> elements)
        {
            if (elements != null && elements.Any())
            {
                root.Add(elements);
            }
        }

        public static void AddIfNotNull(this XElement root, XElement element)
        {
            if (element != default(XElement) && !element.IsEmpty)
            {
                root.Add(element);
            }
        }
    }
}
