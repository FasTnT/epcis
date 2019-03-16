using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Utils;
using MoreLinq;

namespace FasTnT.Formatters.Json
{
    public class JsonEventParser
    {
        private int _internalCounter = 0;

        public IEnumerable<EpcisEvent> Parse(IList<object> list)
        {
            return list.Cast<IDictionary<string, object>>().Select(Parse);
        }
        
        public EpcisEvent Parse(IDictionary<string, object> eventDict)
        {
            var epcisEvent = new EpcisEvent();
            foreach(var key in eventDict.Keys)
            {
                switch (key)
                {
                    case "isA": epcisEvent.Type = Enumeration.GetByDisplayName<EventType>(eventDict[key].ToString()); break;
                    case "eventTime": epcisEvent.EventTime = DateTime.Parse(eventDict[key].ToString()); break;
                    case "eventTimeZoneOffset": epcisEvent.EventTimeZoneOffset = new TimeZoneOffset { Representation = eventDict[key].ToString() }; break;
                    case "action": epcisEvent.Action = Enumeration.GetByDisplayName<EventAction>(eventDict[key].ToString()); break;
                    case "epcList": ParseEpcs(epcisEvent, eventDict[key] as string[], EpcType.List); break;
                    case "childEPCs": ParseChildEpcsInto(eventDict[key] as IList<string>, epcisEvent); break;
                    case "inputQuantityList": ParceQuantityList(epcisEvent, eventDict[key] as IList<object>, EpcType.InputQuantity); break;
                    case "inputEPCList": ParseEpcs(epcisEvent, eventDict[key] as IList<object>, EpcType.InputEpc); break; 
                    case "outputQuantityList": ParceQuantityList(epcisEvent, eventDict[key] as IList<object>, EpcType.OutputQuantity); break;
                    case "outputEPCList": ParseEpcs(epcisEvent, eventDict[key] as IList<object>, EpcType.OutputEpc); break;
                    case "epcClass": epcisEvent.Epcs.Add(new Epc { Type = EpcType.Quantity, Id = eventDict[key].ToString(), IsQuantity = true }); break;
                    case "quantity": epcisEvent.Epcs.Single(x => x.Type == EpcType.Quantity).Quantity = float.Parse(eventDict[key].ToString(), CultureInfo.InvariantCulture); break;
                    case "quantityList": ParseQuantityList(epcisEvent, eventDict[key] as IList<object>); break;
                    case "bizStep": epcisEvent.BusinessStep = eventDict[key].ToString(); break;
                    case "disposition": epcisEvent.Disposition = eventDict[key].ToString(); break;
                    case "eventID": epcisEvent.EventId = eventDict[key].ToString(); break;
                    case "errorDeclaration": break; // TODO: implement.
                    case "transformationId": epcisEvent.TransformationId = eventDict[key].ToString(); break;
                    case "bizLocation": epcisEvent.BusinessLocation = eventDict[key].ToString(); break;
                    case "bizTransactionList": ParseBusinessTransactions(epcisEvent, eventDict[key] as IList<object>); break;
                    case "readPoint": epcisEvent.ReadPoint = eventDict[key].ToString(); break;
                    case "sourceList": ParseSourceDest(epcisEvent, SourceDestinationType.Source, eventDict[key] as IList<object>); break;
                    case "destinationList": ParseSourceDest(epcisEvent, SourceDestinationType.Destination, eventDict[key] as IList<object>); break;
                    case "ilmd": ParseIlmd(epcisEvent, eventDict[key] as IDictionary<string, object>); break;
                    case "parentID": epcisEvent.Epcs.Add(new Epc { Id = eventDict[key].ToString(), Type = EpcType.ParentId }); break;
                    case "recordTime": break; // We don't process record time as it will be overrided in any case..
                    default: TryParseCustomField(epcisEvent, FieldType.EventExtension, key, eventDict[key] as IDictionary<string, object>); break;
                }
            }

            return epcisEvent;
        }

        private void ParseIlmd(EpcisEvent epcisEvent, IDictionary<string, object> dict)
        {
            if (dict == null || !dict.Keys.Any()) return;

            foreach (var key in dict.Keys)
            {
                TryParseCustomField(epcisEvent, FieldType.Ilmd, key, dict[key] as IDictionary<string, object>);
            }
        }

        private void ParseBusinessTransactions(EpcisEvent epcisEvent, IList<object> list)
        {
            if (list == null || !list.Any()) return;

            list.Cast<IDictionary<string, object>>().ForEach(x => epcisEvent.BusinessTransactions.Add(new BusinessTransaction
            {
                Id = x["bizTransaction"].ToString(),
                Type = x["type"].ToString()
            }));
        }

        private void ParceQuantityList(EpcisEvent epcisEvent, IList<object> list, EpcType type)
        {
            if (list == null || !list.Any()) return;

            list.Cast<IDictionary<string, object>>().ForEach(qty => epcisEvent.Epcs.Add(new Epc
            {
                Id = qty["epcClass"].ToString(),
                IsQuantity = true,
                Type = type,
                UnitOfMeasure = qty.ContainsKey("uom") ? qty["uom"].ToString() : null,
                Quantity = int.Parse(qty["quantity"].ToString())
            }));
        }

        private void ParseQuantityList(EpcisEvent epcisEvent, IList<object> list)
        {
            if (list == null || !list.Any()) return;

            list.Cast<IDictionary<string, object>>().ForEach(qty => epcisEvent.Epcs.Add(new Epc
            {
                Id = qty["epcClass"].ToString(),
                IsQuantity = true,
                Type = EpcType.Quantity,
                UnitOfMeasure = qty.ContainsKey("uom") ? qty["uom"].ToString() : null,
                Quantity = int.Parse(qty["quantity"].ToString())
            }));
        }

        private void ParseSourceDest(EpcisEvent epcisEvent, SourceDestinationType direction, IList<object> dictionary)
        {
            if (dictionary == null || !dictionary.Any()) return;

            dictionary.Cast<IDictionary<string, object>>().ForEach(values => epcisEvent.SourceDestinationList.Add(new SourceDestination
            {
                Type = values["type"].ToString(),
                Id = values[direction.DisplayName].ToString(),
                Direction = direction
            }));
        }

        private void ParseEpcs(EpcisEvent epcisEvent, IList<object> epcs, EpcType type)
        {
            if (epcs == null || !epcs.Any()) return;

            epcs.Cast<string>().ForEach(e => epcisEvent.Epcs.Add(new Epc { Id = e, Type = type }));
        }

        private void ParseChildEpcsInto(IList<string> list, EpcisEvent epcisEvent)
        {
            list.ForEach(epc => epcisEvent.Epcs.Add(new Epc { Id = epc, Type = EpcType.ChildEpc }));
        }

        private void TryParseCustomField(EpcisEvent epcisEvent, FieldType type, string id, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) throw new Exception($"Element with name '{id}' is not expected here");

            var namespaceName = GetCustomFieldNamespace(id, dictionary);
            var namespaceValue = dictionary[namespaceName].ToString();
            var value = dictionary["#text"].ToString();

            epcisEvent.CustomFields.Add(new CustomField
            {
                Id = _internalCounter++,
                Namespace = namespaceValue,
                Name = id.Split(':').Last(),
                TextValue = value,
                Type = type
            });

            foreach (var key in dictionary.Keys.Where(k => k.StartsWith("@") && !string.Equals(k, namespaceName)))
            {
                epcisEvent.CustomFields.Add(new CustomField
                {
                    ParentId = _internalCounter - 1,
                    Id = _internalCounter++,
                    Name = key.Substring(1),
                    Namespace = namespaceValue,
                    TextValue = dictionary[key].ToString(),
                    Type = FieldType.Attribute
                });
            }
        }

        private static string GetCustomFieldNamespace(string id, IDictionary<string, object> dictionary)
        {
            var potentialNamespaces = new[] { $"@{id.Split(':')[0]}", $"@xmlns:{id.Split(':')[0]}", "@xmlns" };

            return potentialNamespaces.FirstOrDefault(n => dictionary.ContainsKey(n)) ?? throw new Exception($"Custom event field must have a custom namespace");
        }
    }
}
