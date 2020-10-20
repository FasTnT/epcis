using FasTnT.Model.Enums;
using FasTnT.Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace FasTnT.Parsers.Xml.Formatters
{
    public class XmlEventFormatter
    {
        static readonly IDictionary<EventType, Func<EpcisEvent, XElement>> Formatters = new Dictionary<EventType, Func<EpcisEvent, XElement>>
        {
            { EventType.Object, FormatObjectEvent },
            { EventType.Quantity, FormatQuantityEvent },
            { EventType.Aggregation, FormatAggregationEvent },
            { EventType.Transaction, FormatTransactionEvent },
            { EventType.Transformation, FormatTransformationEvent },
        };

        public static IEnumerable<XElement> FormatList(IList<EpcisEvent> eventList, CancellationToken cancellationToken)
        {
            return eventList.TakeWhile(_ => !cancellationToken.IsCancellationRequested).Select(FormatEvent);
        }

        private static XElement FormatEvent(EpcisEvent evt)
        {
            return Formatters.TryGetValue(evt.Type, out Func<EpcisEvent, XElement> formatter)
                    ? formatter(evt)
                    : throw new Exception($"Unknown event type to format {evt?.Type?.DisplayName}");
        }

        private static XElement FormatObjectEvent(EpcisEvent evt)
        {
            var xmlEvent = new XElement("ObjectEvent");

            AddCommonEventFields(evt, xmlEvent);
            xmlEvent.Add(CreateEpcList(evt, EpcType.List, "epcList"));
            xmlEvent.Add(new XElement("action", evt.Action.DisplayName)); 
            AddV1_1Fields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
            xmlEvent.AddIfNotNull(FormatObjectEventExtension(evt));
            xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

            return xmlEvent;
        }

        private static XElement FormatObjectEventExtension(EpcisEvent evt)
        {
            var extension = new XElement("extension");
            extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
            extension.AddIfNotNull(CreateSourceList(evt));
            extension.AddIfNotNull(CreateDestinationList(evt));
            extension.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
            extension.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));

            return extension;
        }

        private static XElement FormatQuantityEvent(EpcisEvent evt)
        {
            var xmlEvent = new XElement("QuantityEvent");

            AddCommonEventFields(evt, xmlEvent);
            xmlEvent.Add(new XElement("epcClass", evt.Epcs.Single().Id));
            xmlEvent.Add(new XElement("quantity", evt.Epcs.Single().Quantity));
            AddV1_1Fields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
            xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
            xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

            return xmlEvent;
        }

        private static void AddV1_1Fields(EpcisEvent evt, XElement xmlEvent)
        {
            xmlEvent.AddIfNotNull(new XElement("bizStep", evt.BusinessStep));
            xmlEvent.AddIfNotNull(new XElement("disposition", evt.Disposition));
            xmlEvent.AddIfNotNull(CreateReadPoint(evt));
            xmlEvent.AddIfNotNull(CreateBusinessLocation(evt));
        }

        private static XElement FormatAggregationEvent(EpcisEvent evt)
        {
            var xmlEvent = new XElement("AggregationEvent");

            AddCommonEventFields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
            xmlEvent.Add(CreateEpcList(evt, EpcType.ChildEpc, "childEPCs"));
            xmlEvent.Add(new XElement("action", evt.Action.DisplayName));
            AddV1_1Fields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
            xmlEvent.AddIfNotNull(FormatAggregationEventExtension(evt));
            xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

            return xmlEvent;
        }

        private static XElement FormatAggregationEventExtension(EpcisEvent evt)
        {
            var extension = new XElement("extension");
            extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "childQuantityList"));
            extension.AddIfNotNull(CreateSourceList(evt));
            extension.AddIfNotNull(CreateDestinationList(evt));
            extension.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));

            return extension;
        }

        private static XElement FormatTransactionEvent(EpcisEvent evt)
        {
            var xmlEvent = new XElement("TransactionEvent");

            AddCommonEventFields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
            xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
            xmlEvent.Add(CreateEpcList(evt, EpcType.List, "epcList"));
            xmlEvent.Add(new XElement("action", evt.Action.DisplayName));
            AddV1_1Fields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(FormatTransactionEventExtension(evt));
            xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

            return xmlEvent;
        }

        private static XElement FormatTransactionEventExtension(EpcisEvent evt)
        {
            var extension = new XElement("extension");
            extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
            extension.AddIfNotNull(CreateSourceList(evt));
            extension.AddIfNotNull(CreateDestinationList(evt));
            extension.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));

            return extension;
        }

        private static XElement FormatTransformationEvent(EpcisEvent evt)
        {
            var xmlEvent = new XElement("TransformationEvent");

            AddCommonEventFields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateEpcList(evt, EpcType.InputEpc, "inputEPCList"));
            xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.InputQuantity, "inputQuantityList"));
            xmlEvent.AddIfNotNull(CreateEpcList(evt, EpcType.OutputEpc, "outputEPCList"));
            xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.OutputQuantity, "outputQuantityList"));
            xmlEvent.AddIfNotNull(new XElement("transformationID", evt.TransformationId));
            AddV1_1Fields(evt, xmlEvent);
            xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
            xmlEvent.AddIfNotNull(CreateSourceList(evt));
            xmlEvent.AddIfNotNull(CreateDestinationList(evt));
            xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
            xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
            xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

            return new XElement("extension", xmlEvent);
        }

        private static XElement CreateReadPoint(EpcisEvent evt)
        {
            var readPointElement = new XElement("readPoint");
            readPointElement.AddIfNotNull(new XElement("id", evt.ReadPoint));
            readPointElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.ReadPointExtension, "extension"));
            readPointElement.AddIfNotNull(CreateCustomFields(evt, FieldType.ReadPointCustomField));

            return readPointElement;
        }

        private static XElement CreateBusinessLocation(EpcisEvent evt)
        {
            var locationElement = new XElement("bizLocation");
            locationElement.AddIfNotNull(new XElement("id", evt.BusinessLocation));
            locationElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.BusinessLocationExtension, "extension"));
            locationElement.AddIfNotNull(CreateCustomFields(evt, FieldType.BusinessLocationCustomField));

            return locationElement;
        }

        private static XElement CreateSourceList(EpcisEvent evt)
        {
            return new XElement("sourceList", evt.SourceDestinationList
                .Where(x => x.Direction == SourceDestinationType.Source)
                .Select(x => new XElement("source", new XAttribute("type", x.Type), x.Id)));
        }

        private static XElement CreateDestinationList(EpcisEvent evt)
        {
            return new XElement("destinationList", evt.SourceDestinationList
                .Where(x => x.Direction == SourceDestinationType.Destination)
                .Select(x => new XElement("destination", new XAttribute("type", x.Type), x.Id)));
        }

        private static XElement CreateFromCustomFields(EpcisEvent evt, FieldType type, string elementName)
        {
            var extension = new XElement(elementName);
            extension.AddIfNotNull(CreateCustomFields(evt, type));

            return extension;
        }

        private static XElement CreateBizTransactions(EpcisEvent evt)
        {
            var list = new XElement("bizTransactionList");

            list.AddIfNotNull(evt.BusinessTransactions.Select(x => new XElement("bizTransaction", new XAttribute("type", x.Type), x.Id)));

            return list;
        }

        private static XElement CreateEpcList(EpcisEvent evt, EpcType type, string elementName)
        {
            var epcs = evt.Epcs.Where(x => x.Type == type);
            var list = new XElement(elementName);

            list.AddIfNotNull(epcs.Select(x => new XElement("epc", x.Id)));

            return list;
        }

        private static XElement CreateQuantityList(EpcisEvent evt, EpcType type, string elementName)
        {
            var epcs = evt.Epcs.Where(x => x.Type == type);
            var list = new XElement(elementName);

            list.AddIfNotNull(epcs.Select(x =>
            {
                var element = new XElement("quantityElement", new XElement("epcClass", x.Id));
                element.AddIfNotNull(new[] { new XElement("quantity", x.Quantity), new XElement("uom", x.UnitOfMeasure) });

                return element;
            }));

            return list;
        }

        private static void AddCommonEventFields(EpcisEvent evt, XElement xmlEvent)
        {
            xmlEvent.Add(new XElement("eventTime", evt.EventTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            xmlEvent.Add(new XElement("recordTime", evt.CaptureTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            xmlEvent.Add(new XElement("eventTimeZoneOffset", evt.EventTimeZoneOffset.Representation));
            xmlEvent.AddIfNotNull(CreateBaseExtension(evt));
        }

        private static XElement CreateBaseExtension(EpcisEvent evt)
        {
            var baseExtension = new XElement("baseExtension");

            baseExtension.AddIfNotNull(new XElement("eventID", evt.EventId));
            baseExtension.AddIfNotNull(CreateErrorDeclaration(evt));
            baseExtension.AddIfNotNull(CreateCustomFields(evt, FieldType.BaseExtension));

            return baseExtension;
        }

        private static XElement CreateErrorDeclaration(EpcisEvent evt)
        {
            var errorDeclaration = new XElement("errorDeclaration");

            errorDeclaration.AddIfNotNull(new XElement("declarationTime", evt.CorrectiveDeclarationTime));
            errorDeclaration.AddIfNotNull(new XElement("reason", evt.CorrectiveReason));
            errorDeclaration.AddIfNotNull(new XElement("correctiveEventIDs", evt.CorrectiveEventIds.Select(x => new XElement("correctiveEventID", x))));

            return errorDeclaration;
        }

        private static IEnumerable<XElement> CreateCustomFields(EpcisEvent evt, FieldType type)
        {
            return evt.CustomFields.Where(x => x.Type == type).Select(FormatField);
        }

        private static XElement FormatField(CustomField field)
        {
            var element = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);
            element.AddIfNotNull(field.Children.Where(x => x.Type != FieldType.Attribute).Select(FormatField));
            
            foreach(var attribute in field.Children.Where(x => x.Type == FieldType.Attribute))
            {
                element.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
            }

            return element;
        }
    }
}
