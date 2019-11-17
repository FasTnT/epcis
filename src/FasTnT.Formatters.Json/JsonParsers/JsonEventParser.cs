using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Utils;
using MoreLinq;
using Newtonsoft.Json.Linq;

namespace FasTnT.Formatters.Json
{
    public class JsonEventParser
    {
        public static IDictionary<string, Action<EpcisEvent, JToken>> ParserMethods = new Dictionary<string, Action<EpcisEvent, JToken>>
        {
            { "isA", (evt, tok) => evt.Type = Enumeration.GetByDisplayName<EventType>(tok.ToString()) },
            { "eventTime", (evt, tok) => evt.EventTime = DateTime.Parse(tok.ToString()) },
            { "eventTimeZoneOffset", (evt, tok) => evt.EventTimeZoneOffset = new TimeZoneOffset { Representation = tok.ToString() } },
            { "action", (evt, tok) => evt.Action = Enumeration.GetByDisplayName<EventAction>(tok.ToString()) },
            { "epcList", (evt, tok) => ParseEpcs(evt, tok.ToObject<string[]>(), EpcType.List) },
            { "childEPCs", (evt, tok) => ParseChildEpcsInto(tok.ToObject<string[]>(), evt) },
            { "inputQuantityList", (evt, tok) => ParseQuantityList(evt, tok.ToObject<JToken[]>(), EpcType.InputQuantity) },
            { "inputEPCList", (evt, tok) => ParseEpcs(evt, tok.ToObject<string[]>(), EpcType.InputEpc) },
            { "outputQuantityList", (evt, tok) => ParseQuantityList(evt, tok.ToObject<JToken[]>(), EpcType.OutputQuantity) },
            { "outputEPCList", (evt, tok) => ParseEpcs(evt, tok.ToObject<string[]>(), EpcType.OutputEpc) },
            { "childQuantityList", (evt, tok) => ParseQuantityList(evt, tok.ToObject<JToken[]>(), EpcType.ChildQuantity) },
            { "epcClass", (evt, tok) => evt.Epcs.Add(new Epc { Type = EpcType.Quantity, Id = tok.ToString(), IsQuantity = true }) },
            { "quantity", (evt, tok) => evt.Epcs.Single(x => x.Type == EpcType.Quantity).Quantity = float.Parse(tok.ToString(), CultureInfo.InvariantCulture) },
            { "quantityList", (evt, tok) => ParseQuantityList(evt, tok.ToObject<JToken[]>()) },
            { "bizStep", (evt, tok) => evt.BusinessStep = tok.ToString() },
            { "disposition", (evt, tok) => evt.Disposition = tok.ToString() },
            { "eventID", (evt, tok) => evt.EventId = tok.ToString() },
            { "errorDeclaration", (evt, tok) => { } }, // TODO
            { "transformationId", (evt, tok) => evt.TransformationId = tok.ToString() },
            { "bizLocation", (evt, tok) => evt.BusinessLocation = tok.ToString() },
            { "bizTransactionList", (evt, tok) => ParseBusinessTransactions(evt, tok.ToObject<IList<JToken>>()) },
            { "readPoint", (evt, tok) => evt.ReadPoint = tok.ToString() },
            { "sourceList", (evt, tok) => ParseSourceDest(evt, SourceDestinationType.Source, tok.ToObject<JToken[]>()) },
            { "destinationList", (evt, tok) => ParseSourceDest(evt, SourceDestinationType.Destination, tok.ToObject<JToken[]>()) },
            { "ilmd", (evt, tok) => ParseIlmd(evt, tok.ToObject<IDictionary<string, JToken>>()) },
            { "parentID", (evt, tok) => evt.Epcs.Add(new Epc { Id = tok.ToString(), Type = EpcType.ParentId }) },
            { "recordTime", (evt, tok) => { } }, // We don't process record time as it will be overrided in any case..
        };

        public IEnumerable<EpcisEvent> Parse(IEnumerable<JObject> events)
        {
            return events.Select(Parse);
        }
        
        public EpcisEvent Parse(IDictionary<string, JToken> eventDict)
        {
            var epcisEvent = new EpcisEvent();
            foreach(var key in eventDict.Keys)
            {
                if(ParserMethods.TryGetValue(key, out Action<EpcisEvent, JToken> parseMethod))
                {
                    parseMethod(epcisEvent, eventDict[key]);
                }
                else
                { 
                    TryParseCustomField(epcisEvent, FieldType.EventExtension, key, eventDict[key].ToObject<IDictionary<string, JToken>>());
                }
            }

            return epcisEvent;
        }

        private static void ParseIlmd(EpcisEvent epcisEvent, IDictionary<string, JToken> dict)
        {
            if (dict == null || !dict.Keys.Any()) return;

            foreach (var key in dict.Keys)
            {
                TryParseCustomField(epcisEvent, FieldType.Ilmd, key, dict[key] as IDictionary<string, JToken>);
            }
        }

        private static void ParseBusinessTransactions(EpcisEvent epcisEvent, IList<JToken> list)
        {
            if (list == null || !list.Any()) return;

            list.Select(x => x.ToObject<IDictionary<string, object>>()).ForEach(x => epcisEvent.BusinessTransactions.Add(new BusinessTransaction
            {
                Id = x["bizTransaction"].ToString(),
                Type = x["type"].ToString()
            }));
        }

        private static void ParseQuantityList(EpcisEvent epcisEvent, IList<JToken> list, EpcType type)
        {
            if (list == null || !list.Any()) return;

            list.Select(x => x.ToObject<IDictionary<string, JObject>>()).ForEach(qty => epcisEvent.Epcs.Add(new Epc
            {
                Id = qty["epcClass"].ToString(),
                IsQuantity = true,
                Type = type,
                UnitOfMeasure = qty.ContainsKey("uom") ? qty["uom"].ToString() : null,
                Quantity = int.Parse(qty["quantity"].ToString())
            }));
        }

        private static void ParseQuantityList(EpcisEvent epcisEvent, IList<JToken> list)
        {
            if (list == null || !list.Any()) return;

            list.Select(x => x.ToObject<IDictionary<string, JObject>>()).ForEach(qty => epcisEvent.Epcs.Add(new Epc
            {
                Id = qty["epcClass"].ToString(),
                IsQuantity = true,
                Type = EpcType.Quantity,
                UnitOfMeasure = qty.ContainsKey("uom") ? qty["uom"].ToString() : null,
                Quantity = int.Parse(qty["quantity"].ToString())
            }));
        }

        private static void ParseSourceDest(EpcisEvent epcisEvent, SourceDestinationType direction, IList<JToken> dictionary)
        {
            if (dictionary == null || !dictionary.Any()) return;

            dictionary.Select(x => x.ToObject<IDictionary<string, JObject>>()).ForEach(values => epcisEvent.SourceDestinationList.Add(new SourceDestination
            {
                Type = values["type"].ToString(),
                Id = values[direction.DisplayName].ToString(),
                Direction = direction
            }));
        }

        private static void ParseEpcs(EpcisEvent epcisEvent, IList<string> epcs, EpcType type)
        {
            if (epcs == null || !epcs.Any()) return;

            epcs.ForEach(e => epcisEvent.Epcs.Add(new Epc { Id = e, Type = type }));
        }

        private static void ParseChildEpcsInto(IList<string> list, EpcisEvent epcisEvent)
        {
            list.ForEach(epc => epcisEvent.Epcs.Add(new Epc { Id = epc, Type = EpcType.ChildEpc }));
        }

        private static void TryParseCustomField(EpcisEvent epcisEvent, FieldType type, string id, IDictionary<string, JToken> dictionary)
        {
            if (dictionary == null) throw new Exception($"Element with name '{id}' is not expected here");

            var namespaceName = GetCustomFieldNamespace(id, dictionary);
            var namespaceValue = dictionary[namespaceName].ToString();
            var value = dictionary["#text"].ToString();

            var customField = new CustomField
            {
                Namespace = namespaceValue,
                Name = id.Split(':').Last(),
                TextValue = value,
                Type = type
            };

            foreach (var key in dictionary.Keys.Where(k => k.StartsWith("@") && !string.Equals(k, namespaceName)))
            {
                customField.Children.Add(new CustomField
                {
                    Name = key.Substring(1),
                    Namespace = namespaceValue,
                    TextValue = dictionary[key].ToString(),
                    Type = FieldType.Attribute
                });
            }

            epcisEvent.CustomFields.Add(customField);
        }

        private static string GetCustomFieldNamespace(string id, IDictionary<string, JToken> dictionary)
        {
            var potentialNamespaces = new[] { $"@{id.Split(':')[0]}", $"@xmlns:{id.Split(':')[0]}", "@xmlns" };

            return potentialNamespaces.FirstOrDefault(n => dictionary.ContainsKey(n)) ?? throw new Exception($"Custom event field must have a custom namespace");
        }
    }
}
