using System.Collections.Generic;
using System.Threading.Tasks;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Queries.Enums;

namespace FasTnT.Domain.Persistence
{
    public interface IEventRepository
    {
        void SetLimit(int eventLimit);
        void OrderBy(EpcisField field, OrderDirection direction);
        void WhereSimpleFieldIn<T>(EpcisField field, T[] values);
        void WhereSimpleFieldMatches(EpcisField field, FilterComparator filterOperator, object value);
        void WhereBusinessTransactionValueIn(string txName, string[] txValues);
        void WhereSourceDestinationValueIn(string sourceName, SourceDestinationType type, string[] sourceValues);
        void WhereEpcMatches(string[] values, EpcType[] epcTypes);
        void WhereExistsErrorDeclaration();
        void WhereErrorReasonIn(string[] errorReasons);
        void WhereCorrectiveEventIdIn(string[] correctiveEventIds);
        void WhereMasterDataHierarchyContains(EpcisField businessLocation, string[] values);
        void WhereCustomFieldMatches(bool inner, FieldType fieldType, string fieldNamespace, string fieldName, string[] values);
        void WhereCustomFieldMatches(bool inner, FieldType fieldType, string fieldNamespace, string fieldName, FilterComparator comparator, object value);
        void WhereCustomFieldExists(bool inner, FieldType fieldType, string fieldNamespace, string fieldName);
        void WhereEpcQuantityMatches(FilterComparator filterOperator, double value);

        Task<IEnumerable<EpcisEvent>> ToList();
    }
}
