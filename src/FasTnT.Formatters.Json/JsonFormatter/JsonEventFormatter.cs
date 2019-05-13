using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Formatters.Json.JsonFormatter
{
    public class JsonEventFormatter
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public IDictionary<string, object> FormatEvent(EpcisEvent evt)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "isA", evt.Type.DisplayName },
                { "recordTime", evt.CaptureTime.ToString(DateTimeFormat) },
                { "eventTime", evt.EventTime.ToString(DateTimeFormat) },
                { "eventTimeZoneOffset", evt.EventTimeZoneOffset.Representation }
            };

            AddEpcs(evt, dictionary);
            AddIfNotNull(evt, dictionary, "action", x => x.Action, x => x.Action.DisplayName);
            AddIfNotNull(evt, dictionary, "bizStep", x => x.BusinessStep);
            AddIfNotNull(evt, dictionary, "disposition", x => x.Disposition);
            AddIfNotNull(evt, dictionary, "readPoint", x => x.ReadPoint);
            AddIfNotNull(evt, dictionary, "bizLocation", x => x.BusinessLocation);
            AddSourceDestinationList(evt, dictionary);
            AddCustomFields(evt.CustomFields, dictionary);

            return dictionary;
        }

        private void AddEpcs(EpcisEvent evt, Dictionary<string, object> dictionary)
        {
            var epcs = evt.Epcs;
            var list = epcs.Where(x => x.Type == EpcType.List).Select(x => x.Id);
            var qtyList = epcs.Where(x => x.Type == EpcType.Quantity).Select(FormatQuantity);
            var inputEpc = epcs.Where(x => x.Type == EpcType.InputEpc).Select(x => x.Id);
            var inputQty = epcs.Where(x => x.Type == EpcType.InputQuantity).Select(FormatQuantity);
            var outputEpc = epcs.Where(x => x.Type == EpcType.OutputEpc).Select(x => x.Id);
            var outputQty = epcs.Where(x => x.Type == EpcType.OutputQuantity).Select(FormatQuantity);
            var childQty = epcs.Where(x => x.Type == EpcType.ChildQuantity).Select(FormatQuantity);


            if (list.Any()) dictionary.Add("epcList", list);
            if (qtyList.Any()) dictionary.Add("quantityList", qtyList);
            if (inputEpc.Any()) dictionary.Add("inputEpcList", inputEpc);
            if (inputQty.Any()) dictionary.Add("inputQuantityList", inputQty);
            if (outputEpc.Any()) dictionary.Add("outputEpcList", outputEpc);
            if (outputQty.Any()) dictionary.Add("outputQuantityList", outputQty);
            if (childQty.Any()) dictionary.Add("childQuantityList", childQty);
        }

        private object FormatQuantity(Epc x)
        {
            return new Dictionary<string, object>
            {
                { "epcClass", x.Id },
                { "quantity", x.Quantity },
                { "uom", x.UnitOfMeasure }
            };
        }

        private void AddSourceDestinationList(EpcisEvent evt, Dictionary<string, object> dictionary)
        {
            var sources = evt.SourceDestinationList.Where(x => x.Direction == SourceDestinationType.Source).Select(s => new Dictionary<string, object>
            {
                { "type", s.Type },
                { "source", s.Id }
            });
            var dests = evt.SourceDestinationList.Where(x => x.Direction == SourceDestinationType.Destination).Select(s => new Dictionary<string, object>
            {
                { "type", s.Type },
                { "destination", s.Id }
            });

            if (sources.Any()) dictionary.Add("sourceList", sources);
            if (dests.Any()) dictionary.Add("destinationList", dests);
        }

        private void AddCustomFields(IEnumerable<CustomField> fields, Dictionary<string, object> dictionary)
        {
            foreach (var field in fields)
            {
                var customField = new Dictionary<string, object>
                {
                    { "@xmlns", field.Namespace },
                    { "#text", (object)field.NumericValue ?? field.TextValue }
                };

                foreach(var attribute in field.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    customField.Add($"@{attribute.Name}", (object)attribute.NumericValue ?? attribute.TextValue);
                }
                AddCustomFields(field.Children.Where(x => x.Type != FieldType.Attribute), customField);

                dictionary.Add(field.Name, customField);
            }
        }

        private void AddIfNotNull(EpcisEvent evt, IDictionary<string, object> formatted, string key, Func<EpcisEvent, object> selector, Func<EpcisEvent, string> value = null)
        {
            if(selector(evt) != null)
            {
                formatted.Add(key, value == null ? selector(evt) : value(evt));
            }
        }
    }
}
