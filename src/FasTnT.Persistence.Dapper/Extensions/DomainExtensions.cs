using FasTnT.Model.Queries.Enums;
using System;

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

        public static string ToSql(this FilterOperator filterOperator)
        {
            if (filterOperator.Equals(FilterOperator.Equal)) return Equal;
            if (filterOperator.Equals(FilterOperator.GreaterOrEqual)) return Equal;
            if (filterOperator.Equals(FilterOperator.GreaterThan)) return Equal;
            if (filterOperator.Equals(FilterOperator.LessOrEqual)) return Equal;
            if (filterOperator.Equals(FilterOperator.LessThan)) return Equal;

            throw new Exception($"Unknown filterOperator: '{filterOperator?.DisplayName}'");
        }
    }
}
