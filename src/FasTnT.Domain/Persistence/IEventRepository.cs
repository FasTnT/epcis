using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FasTnT.Model.Queries.Enums;

namespace FasTnT.Domain.Services.Handlers.PredefinedQueries
{
    public interface IEventRepository
    {
        void SetMaxEventCount(int maxEventCount);
        void SetEventLimit(int eventLimit);
        void WhereRequestIdIn(params Guid[] requestIds);
        void WhereEventIdIn(params string[] eventIds);
        void WhereActionIn(params EventAction[] actions);
        void WhereBusinessStepIn(params string[] businessSteps);
        void WhereDispositionIn(params string[] dispositions);
        void WhereReadPointIn(params string[] readPoints);
        void WhereBusinessTransactionValueIn(string txName, params string[] txValues);
        void WhereBusinessLocationIn(params string[] businessLocations);
        void WhereSourceValueIn(string sourceName, params string[] sourceValues);
        void WhereDestinationValueIn(string destName, params string[] destValues);
        void WhereTransformationIdIn(params string[] transformationIds);
        void WhereEpcMatches(string[] values, EpcType epcType = null);
        void WhereExistsErrorDeclaration();
        void WhereErrorReasonIn(params string[] errorReasons);
        void WhereCorrectiveEventIdIn(params string[] correctiveEventIds);
        void WhereMatchesIlmd<T>(bool inner, string ilmdNamespace, string ilmdName, string comparator, T values);
        void WhereExistsIlmd(bool inner, string ilmdNamespace, string ilmdName);
        void WhereMatchesCustomField<T>(bool inner, string fieldNamespace, string fieldName, string comparator, T values);
        void WhereExistsCustomField(bool inner, string fieldNamespace, string fieldName);
        void WhereCaptureTimeMatches(FilterOperator filterOperator, DateTime dateTime);
        void WhereRecordTimeMatches(FilterOperator filterOperator, DateTime dateTime);

        Task<IEnumerable<EpcisEvent>> ToList();
    }
}
