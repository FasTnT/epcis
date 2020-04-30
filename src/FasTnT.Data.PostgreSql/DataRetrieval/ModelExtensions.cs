using FasTnT.Model.Enums;
using FasTnT.Model.Utils;
using System;
using Mapping = System.Collections.Generic.Dictionary<FasTnT.Model.Utils.Enumeration, string>;

namespace FasTnT.Data.PostgreSql.DataRetrieval
{
    public static class ModelExtensions
    {
        private static readonly Mapping FilterOperators = new Mapping{
            { FilterComparator.Equal, "=" },
            { FilterComparator.GreaterOrEqual, ">=" },
            { FilterComparator.GreaterThan, ">" },
            { FilterComparator.LessOrEqual, "<=" },
            { FilterComparator.LessThan, "<" },
        };

        private static readonly Mapping SortOperators = new Mapping
        {
            { OrderDirection.Ascending, "ASC" },
            { OrderDirection.Descending, "DESC" }
        };
        private static readonly Mapping CbvTypes = new Mapping
        {
            { EpcisField.BusinessLocation, "urn:epcglobal:epcis:vtype:BusinessLocation" },
            { EpcisField.ReadPoint, "urn:epcglobal:epcis:vtype:ReadPoint" }
        };
        private static readonly Mapping SimpleFields = new Mapping
        {
            { EpcisField.Action, "event.action" },
            { EpcisField.BusinessLocation, "event.business_location" },
            { EpcisField.BusinessStep, "event.business_step" },
            { EpcisField.CaptureTime, "event.record_time" },
            { EpcisField.Disposition, "event.disposition" },
            { EpcisField.EventId, "event.event_id" },
            { EpcisField.RecordTime, "request.record_time" },
            { EpcisField.EventType, "event.event_type" },
            { EpcisField.ReadPoint, "event.read_point" },
            { EpcisField.RequestId, "request.id" },
            { EpcisField.TransformationId, "event.transformation_id" }
        };

        public static string ToSql(this FilterComparator op) => GetValue(op, FilterOperators) ?? throw new Exception($"Unknown filterOperator: '{op?.DisplayName}'");
        public static string ToPgSql(this OrderDirection direction) => GetValue(direction, SortOperators) ?? throw new Exception($"Unknown simple EPCIS event field: '{direction.DisplayName}'");
        public static string ToPgSql(this EpcisField field) => GetValue(field, SimpleFields) ?? throw new Exception($"Unknown simple EPCIS event field: '{field.DisplayName}'");
        public static string ToCbvType(this EpcisField field) => GetValue(field, CbvTypes) ?? throw new Exception($"Cannot convert to CBV type: '{field.DisplayName}'");
        public static string GetCustomFieldName(this object value) => value is DateTime ? "date_value" : "numeric_value";

        private static string GetValue(Enumeration value, Mapping mapping) => mapping.TryGetValue(value, out string result) ? result : null;
    }
}
