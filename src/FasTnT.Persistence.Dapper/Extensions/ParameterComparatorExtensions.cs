using System;
using FasTnT.Model.Queries.PredefinedQueries.Parameters;
using FasTnT.Model.Queries.PredefinedQueries.Parameters.Enums;

namespace FasTnT.Persistence.Dapper
{
    public static class DomainExtensions
    {
        const string GreaterThan = ">";
        const string GreaterThanOrEqual = ">=";
        const string LessThan = "<";
        const string LessThanOrEqual = "<=";
        const string Equal = "=";
        const string Ascending = "ASC";
        const string Descending = "DESC";

        public static string ToSql(this ParameterComparator comparator)
        {
            if(comparator == ParameterComparator.GreaterThan) return GreaterThan;
            if(comparator == ParameterComparator.GreaterThanOrEqual) return GreaterThanOrEqual;
            if(comparator == ParameterComparator.LessThan) return LessThan;
            if(comparator == ParameterComparator.LessThanOrEqual) return LessThanOrEqual;
            if(comparator == ParameterComparator.Equal) return Equal;

            throw new Exception($"Unknown comparator: {comparator.DisplayName}");
        }

        public static string ToSql(this OrderDirection direction)
        {
            if (direction == OrderDirection.Asc) return Ascending;
            if (direction == OrderDirection.Desc) return Descending;

            throw new Exception($"Unknown direction: {direction.DisplayName}");
        }
    }
}
