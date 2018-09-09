using FasTnT.Domain;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Requests
{
    public static class XmlEventsParser
    { 
        private static int _customCounter = 0;

        internal static EpcisEvent[] ParseEvents(params XElement[] eventList)
        {
            var events = new List<EpcisEvent>(eventList.Length);

            foreach(var node in eventList)
            {
                _customCounter = 0;

                if(node.Name.LocalName == "extension")
                {
                    var transformationEvent = ParseEvents(node.Elements().Single()).SingleOrDefault();
                    if (transformationEvent != null) events.Add(transformationEvent);
                }
                else
                {
                    events.Add(ParseAttributes(node, new EpcisEvent { Type = Enumeration.GetByDisplayName<EventType>(node.Name.LocalName) }));
                }
            }

            return events.ToArray();
        }

        private static EpcisEvent ParseAttributes(XElement root, EpcisEvent epcisEvent)
        {
            foreach(var node in root.Elements())
            {
                switch (node.Name.LocalName)
                {
                    case "eventTime":
                        epcisEvent.EventTime = DateTime.Parse(node.Value, CultureInfo.InvariantCulture); break;
                    case "eventTimeZoneOffset":
                        epcisEvent.EventTimeZoneOffset = new TimeZoneOffset { Representation = node.Value }; break;
                    case "action":
                        epcisEvent.Action = Enumeration.GetByDisplayName<EventAction>(node.Value); break;
                    case "epcList":
                        node.ParseEpcListInto(epcisEvent); break;
                    case "childEPCs":
                        node.ParseChildEpcListInto(epcisEvent); break;
                    case "inputQuantityList":
                        node.ParseQuantityListInto(epcisEvent, true); break;
                    case "inputEPCList":
                        node.ParseEpcListInto(epcisEvent, true); break;
                    case "outputQuantityList":
                        node.ParseQuantityListInto(epcisEvent, false); break;
                    case "outputEPCList":
                        node.ParseEpcListInto(epcisEvent, false); break;
                    case "epcClass":
                        epcisEvent.Epcs.Add(new Epc { Type = EpcType.Quantity, Id = node.Value, IsQuantity = true }); break;
                    case "quantity":
                        epcisEvent.Epcs.Single(x => x.Type == EpcType.Quantity).Quantity = float.Parse(node.Value, CultureInfo.InvariantCulture); break;
                    case "bizStep":
                        epcisEvent.BusinessStep = node.Value; break;
                    case "disposition":
                        epcisEvent.Disposition = node.Value; break;
                    case "eventID":
                        epcisEvent.EventId = node.Value; break;
                    case "errorDeclaration":
                        epcisEvent.ErrorDeclaration = node.ToErrorDeclaration(epcisEvent); break;
                    case "transformationId":
                        epcisEvent.TransformationId = node.Value; break;
                    case "bizLocation":
                        node.ParseBusinessLocation(epcisEvent); break;
                    case "bizTransactionList":
                        epcisEvent.BusinessTransactions = node.ToBusinessTransactions(); break;
                    case "readPoint":
                        node.ParseReadPoint(epcisEvent); break;
                    case "sourceList":
                        node.ParseSourceInto(epcisEvent.SourceDestinationList); break;
                    case "destinationList":
                        node.ParseDestinationInto(epcisEvent.SourceDestinationList); break;
                    case "ilmd":
                        ParseIlmd(node, epcisEvent); break;
                    case "parentID":
                        epcisEvent.Epcs.Add(new Epc { Id = node.Value, Type = EpcType.ParentId }); break;
                    case "recordTime": // We don't process record time as it will be overrided in any case..
                        break;
                    case "extension":
                        ParseExtensionElement(node, epcisEvent); break;
                    default:
                        epcisEvent.CustomFields.Add(ParseCustomField(node, epcisEvent, FieldType.EventExtension)); break;
                }
            }

            return epcisEvent;
        }

        private static void ParseExtensionElement(XElement innerElement, EpcisEvent epcisEvent)
        {
            if (innerElement.Name.Namespace == XNamespace.None || innerElement.Name.Namespace == XNamespace.Xmlns || innerElement.Name.NamespaceName == EpcisNamespaces.Capture)
                epcisEvent = ParseAttributes(innerElement, epcisEvent);
            else
                epcisEvent.CustomFields.Add(ParseCustomField(innerElement, epcisEvent, FieldType.EventExtension));
        }

        private static void ParseIlmd(XElement element, EpcisEvent epcisEvent)
        {
            foreach (var children in element.Elements())
            {
                epcisEvent.CustomFields.Add(ParseCustomField(children, epcisEvent, FieldType.Ilmd));
            }
        }

        internal static CustomField ParseCustomField(XElement element, EpcisEvent epcisEvent, FieldType type)
        {
            if (element.Name.Namespace == XNamespace.None || element.Name.Namespace == XNamespace.Xmlns || element.Name.NamespaceName == EpcisNamespaces.Capture)
            {
                throw new Exception($"Element '{element.Name.LocalName}' with namespace '{element.Name.NamespaceName}' not expected here.");
            }

            var field = new CustomField
            {
                Id = _customCounter++,
                Type = type,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                TextValue = element.HasElements ? null : element.Value,
                NumericValue = element.HasElements ? null : double.TryParse(element.Value, out double doubleValue) ? doubleValue : default(double?),
                DateValue = element.HasElements ? null : DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?),
            };

            if (element.HasElements)
            {
                foreach (var children in element.Elements())
                {
                    var childrenField = ParseCustomField(children, epcisEvent, type);
                    childrenField.ParentId = field.Id;
                    epcisEvent.CustomFields.Add(childrenField);
                }
            }

            foreach (var attribute in element.Attributes().Where(x => x.Name.Namespace != XNamespace.None && x.Name.Namespace != XNamespace.Xmlns && x.Name.NamespaceName != EpcisNamespaces.Capture))
            {
                var attributeField = new CustomField
                {
                    Id = _customCounter++,
                    ParentId = field.Id,
                    Type = FieldType.Attribute,
                    Name = attribute.Name.LocalName,
                    Namespace = attribute.Name.Namespace.NamespaceName,
                    TextValue = attribute.Value,
                    NumericValue = element.HasElements ? null : double.TryParse(element.Value, out double doubleVal) ? doubleVal : default(double?),
                    DateValue = element.HasElements ? null : DateTime.TryParse(element.Value, out DateTime dateVal) ? dateVal : default(DateTime?),
                };

                epcisEvent.CustomFields.Add(attributeField);
            }

            return field;
        }
    }
}
