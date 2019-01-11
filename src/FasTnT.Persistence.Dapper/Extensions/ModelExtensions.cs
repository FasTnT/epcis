using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries.Enums;
using System;

namespace FasTnT.Persistence.Dapper
{
    public static class ModelExtensions
    {
        const string GreaterThan = ">";
        const string GreaterThanOrEqual = ">=";
        const string LessThan = "<";
        const string LessThanOrEqual = "<=";
        const string Equal = "=";
        const string Ascending = "ASC";
        const string Descending = "DESC";

        public static string ToSql(this FilterComparator filterOperator)
        {
            if (filterOperator.Equals(FilterComparator.Equal)) return Equal;
            if (filterOperator.Equals(FilterComparator.GreaterOrEqual)) return GreaterThanOrEqual;
            if (filterOperator.Equals(FilterComparator.GreaterThan)) return GreaterThan;
            if (filterOperator.Equals(FilterComparator.LessOrEqual)) return LessThanOrEqual;
            if (filterOperator.Equals(FilterComparator.LessThan)) return LessThan;

            throw new Exception($"Unknown filterOperator: '{filterOperator?.DisplayName}'");
        }

        public static string ToPgSql(this OrderDirection direction)
        {
            if (direction.Equals(OrderDirection.Ascending)) return Ascending;
            if (direction.Equals(OrderDirection.Descending)) return Descending;

            throw new Exception($"Unknown simple EPCIS event field: '{direction.DisplayName}'");
        }

        public static string ToPgSql(this EpcisField field)
        {
            if (field.Equals(EpcisField.Action)) return "event.action";
            if (field.Equals(EpcisField.BusinessLocation)) return "event.business_location";
            if (field.Equals(EpcisField.BusinessStep)) return "event.business_step";
            if (field.Equals(EpcisField.CaptureTime)) return "event.record_time";
            if (field.Equals(EpcisField.Disposition)) return "event.disposition";
            if (field.Equals(EpcisField.EventId)) return "event.id";
            if (field.Equals(EpcisField.RecordTime)) return "request.record_time";
            if (field.Equals(EpcisField.EventType)) return "event.event_type";
            if (field.Equals(EpcisField.ReadPoint)) return "event.read_point";
            if (field.Equals(EpcisField.RequestId)) return "request.id";
            if (field.Equals(EpcisField.TransformationId)) return "event.transformation_id";

            throw new Exception($"Unknown simple EPCIS event field: '{field.DisplayName}'");
        }
    }
}
