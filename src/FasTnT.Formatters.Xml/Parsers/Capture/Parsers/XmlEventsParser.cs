using FasTnT.Model.Enums;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using FasTnT.Parsers.Xml.Utils;
using FasTnT.Model.Events;

namespace FasTnT.Parsers.Xml.Capture
{
    public static class XmlEventsParser
    {
        private static readonly IDictionary<string, Action<EpcisEvent, XElement>> ParserMethods = new Dictionary<string, Action<EpcisEvent, XElement>>
        {
            { "eventTime",           (evt, node) => evt.EventTime = DateTime.Parse(node.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) },
            { "eventTimeZoneOffset", (evt, node) => evt.EventTimeZoneOffset = new TimeZoneOffset { Representation = node.Value } },
            { "action",              (evt, node) => evt.Action = Enumeration.GetByDisplayName<EventAction>(node.Value) },
            { "epcList",             (evt, node) => node.ParseEpcListInto(EpcType.List, evt) },
            { "childEPCs",           (evt, node) => node.ParseChildEpcListInto(evt) },
            { "inputQuantityList",   (evt, node) => node.ParseQuantityListInto(evt, EpcType.InputQuantity) },
            { "inputEPCList",        (evt, node) => node.ParseEpcListInto(EpcType.InputEpc, evt) },
            { "outputQuantityList",  (evt, node) => node.ParseQuantityListInto(evt, EpcType.OutputQuantity) },
            { "childQuantityList",   (evt, node) => node.ParseQuantityListInto(evt, EpcType.ChildQuantity) },
            { "epcClass",            (evt, node) => evt.Epcs.Add(new Epc { Type = EpcType.Quantity, Id = node.Value, IsQuantity = true }) },
            { "quantity",            (evt, node) => evt.Epcs.Single(x => x.Type == EpcType.Quantity).Quantity = float.Parse(node.Value, CultureInfo.InvariantCulture) },
            { "bizStep",             (evt, node) => evt.BusinessStep = node.Value },
            { "disposition",         (evt, node) => evt.Disposition = node.Value },
            { "eventID",             (evt, node) => evt.EventId = node.Value },
            { "errorDeclaration",    (evt, node) => evt.ErrorDeclaration = node.ToErrorDeclaration(evt) },
            { "transformationId",    (evt, node) => evt.TransformationId = node.Value },
            { "bizLocation",         (evt, node) => node.ParseBusinessLocation(evt) },
            { "bizTransactionList",  (evt, node) => evt.BusinessTransactions = node.ToBusinessTransactions() },
            { "readPoint",           (evt, node) => node.ParseReadPoint(evt) },
            { "sourceList",          (evt, node) => node.ParseSourceDest(SourceDestinationType.Source, evt.SourceDestinationList) },
            { "destinationList",     (evt, node) => node.ParseSourceDest(SourceDestinationType.Destination, evt.SourceDestinationList) },
            { "ilmd",                (evt, node) => ParseIlmd(node, evt) },
            { "parentID",            (evt, node) => evt.Epcs.Add(new Epc { Id = node.Value, Type = EpcType.ParentId }) },
            { "recordTime",          (evt, node) => { } }, // We don't process record time as it will be overrided in any case..
            { "extension",           (evt, node) => ParseExtensionElement(node, evt) },
            { "baseExtension",       (evt, node) => ParseExtensionElement(node, evt) },
        };

        internal static List<EpcisEvent> ParseEvents(params XElement[] eventList)
        {
            var events = new List<EpcisEvent>(eventList.Length);

            foreach(var node in eventList)
            {
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

            return events.ToList();
        }

        internal static EpcisEvent ParseAttributes(XElement root, EpcisEvent epcisEvent)
        {
            foreach(var node in root.Elements())
            {
                if (node.Name.NamespaceName != XNamespace.None && node.Name.NamespaceName != XNamespace.Xmlns && node.Name.NamespaceName != EpcisNamespaces.Capture)
                {
                    epcisEvent.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(node, FieldType.CustomField));
                }
                else if (ParserMethods.TryGetValue(node.Name.LocalName, out Action<EpcisEvent, XElement> parserMethod))
                {
                    parserMethod(epcisEvent, node);
                }
                else
                {
                    epcisEvent.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(node, FieldType.EventExtension));
                }
            }

            return epcisEvent;
        }

        internal static void ParseExtensionElement(XElement innerElement, EpcisEvent epcisEvent)
        {
            if (innerElement.Name.Namespace == XNamespace.None || innerElement.Name.Namespace == XNamespace.Xmlns || innerElement.Name.NamespaceName == EpcisNamespaces.Capture)
                _ = ParseAttributes(innerElement, epcisEvent);
            else
                epcisEvent.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(innerElement, FieldType.EventExtension));
        }

        internal static void ParseIlmd(XElement element, EpcisEvent epcisEvent)
        {
            foreach (var children in element.Elements())
            {
                epcisEvent.CustomFields.Add(XmlCustomFieldParser.ParseCustomField(children, FieldType.Ilmd));
            }
        }
    }
}
