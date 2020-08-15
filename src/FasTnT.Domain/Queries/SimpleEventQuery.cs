using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Domain.Utils;
using FasTnT.Model.Enums;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public class SimpleEventQuery : IEpcisQuery
    {
        private static readonly IDictionary<string, Action<IEventFetcher, QueryParameter>> SimpleParameters = new Dictionary<string, Action<IEventFetcher, QueryParameter>>
        {
            { "eventType",               (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<EventType> { Field = EpcisField.EventType, Values = param.Values.Select(Enumeration.GetByDisplayName<EventType>).ToArray() }) },
            { "eventCountLimit",         (fetcher, param) => fetcher.Apply(new LimitFilter { Value = param.GetValue<int>() }) },
            { "maxEventCount",           (fetcher, param) => fetcher.Apply(new LimitFilter { Value = param.GetValue<int>() +1 }) },
            { "EQ_action",               (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<EventAction> { Field = EpcisField.Action, Values = param.Values.Select(Enumeration.GetByDisplayName<EventAction>).ToArray() }) },
            { "EQ_bizLocation",          (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.BusinessLocation, Values =  param.Values }) },
            { "EQ_bizStep",              (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.BusinessStep, Values =  param.Values }) },
            { "EQ_disposition",          (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.Disposition, Values =  param.Values }) },
            { "EQ_eventID",              (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.EventId, Values =  param.Values }) },
            { "EQ_transformationID",     (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.TransformationId, Values =  param.Values }) },
            { "EQ_readPoint",            (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<string> { Field = EpcisField.ReadPoint, Values =  param.Values }) },
            { "EXISTS_errorDeclaration", (fetcher, param) => fetcher.Apply(new ExistsErrorDeclarationFilter ()) },
            { "EQ_errorReason",          (fetcher, param) => fetcher.Apply(new EqualsErrorReasonFilter { Values = param.Values }) },
            { "EQ_correctiveEventID",    (fetcher, param) => fetcher.Apply(new EqualsCorrectiveEventIdFilter { Values = param.Values }) },
            { "WD_readPoint",            (fetcher, param) => fetcher.Apply(new MasterdataHierarchyFilter { Field = EpcisField.ReadPoint, Values = param.Values }) },
            { "WD_bizLocation",          (fetcher, param) => fetcher.Apply(new MasterdataHierarchyFilter { Field = EpcisField.BusinessLocation, Values = param.Values }) },
            { "EQ_requestId",            (fetcher, param) => fetcher.Apply(new SimpleParameterFilter<int> { Field = EpcisField.RequestId, Values = param.Values.Select(int.Parse).ToArray() }) },
            { "orderBy",                 (fetcher, param) => fetcher.Apply(new OrderFilter { Field = Enumeration.GetByDisplayName<EpcisField>(param.GetValue<string>()) }) },
            { "orderDirection",          (fetcher, param) => fetcher.Apply(new OrderDirectionFilter { Direction = Enumeration.GetByDisplayName<OrderDirection>(param.GetValue<string>()) }) }
        };
        private static readonly IDictionary<string, Action<IEventFetcher, QueryParameter>> RegexParameters = new Dictionary<string, Action<IEventFetcher, QueryParameter>>
        {
            { "^EQ_(source|destination)_",  (fetcher, param) => fetcher.Apply(new SourceDestinationFilter { Name = param.Name.Split('_', 3)[2], Type = param.GetSourceDestinationType(), Values = param.Values }) },
            { "^EQ_bizTransaction_",        (fetcher, param) => fetcher.Apply(new BusinessTransactionFilter { TransactionType = param.Name.Split('_', 3)[2], Values = param.Values }) },
            { "^(GE|LT)_eventTime",         (fetcher, param) => fetcher.Apply(new ComparisonParameterFilter { Field = EpcisField.CaptureTime, Comparator = param.GetComparator(), Value = param.GetValue<DateTime>() }) },
            { "^(GE|LT)_recordTime",        (fetcher, param) => fetcher.Apply(new ComparisonParameterFilter { Field = EpcisField.RecordTime, Comparator = param.GetComparator(), Value = param.GetValue<DateTime>() }) },
            { "^MATCH_",                    (fetcher, param) => fetcher.Apply(new MatchEpcFilter { EpcType = param.GetMatchEpcTypes(), Values = param.Values.Select(x => x.Replace("*", "%")).ToArray() }) },
            { "^(EQ|GT|LT|GE|LE)_quantity$",(fetcher, param) => fetcher.Apply(new QuantityFilter { Operator = param.GetComparator(), Value = param.GetValue<double>() }) },
            { "^EQ_INNER_ILMD_",            (fetcher, param) => fetcher.Apply(new CustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), IsInner = true, Values = param.Values }) },
            { "^EQ_ILMD_",                  (fetcher, param) => fetcher.Apply(new CustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), IsInner = false, Values = param.Values }) },
            { "^(GT|LT|GE|LE)_INNER_ILMD_", (fetcher, param) => fetcher.Apply(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), Comparator = param.GetComparator(), IsInner = true, Value = param.GetComparisonValue()  }) },
            { "^(GT|LT|GE|LE)_ILMD_",       (fetcher, param) => fetcher.Apply(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), Comparator = param.GetComparator(), IsInner = false, Value = param.GetComparisonValue()  }) },
            { "^EXISTS_INNER_ILMD_",        (fetcher, param) => fetcher.Apply(new ExistCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), IsInner = true }) },
            { "^EXISTS_ILMD_",              (fetcher, param) => fetcher.Apply(new ExistCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), IsInner = false }) },
            { "^EQ_INNER_",                 (fetcher, param) => fetcher.Apply(new CustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), IsInner = true, Values = param.Values }) },
            { "^EQ_",                       (fetcher, param) => fetcher.Apply(new CustomFieldFilter { Field = param.GetField(FieldType.CustomField, false), IsInner = false, Values = param.Values }) },
            { "^(GT|LT|GE|LE)_INNER_",      (fetcher, param) => fetcher.Apply(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), Comparator = param.GetComparator(), IsInner = true, Value = param.GetComparisonValue() }) },
            { "^(GT|LT|GE|LE)_",            (fetcher, param) => fetcher.Apply(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.CustomField, false), Comparator = param.GetComparator(), IsInner = false, Value = param.GetComparisonValue() }) },
            { "^EXISTS_INNER",              (fetcher, param) => fetcher.Apply(new ExistCustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), IsInner = true }) },
            { "^EQATTR_",                   (fetcher, param) => fetcher.Apply(new AttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName(), Values = param.Values }) },
            { "^HASATTR_",                  (fetcher, param) => fetcher.Apply(new ExistsAttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName()}) }
        };
        private readonly IEventFetcher _eventFetcher;

        public SimpleEventQuery(IEventFetcher eventFetcher)
        {
            _eventFetcher = eventFetcher;
        }

        public string Name => "SimpleEventQuery";
        public bool AllowSubscription => true;

        public async Task<PollResponse> Handle(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var maxEventCount = default(int?);

            foreach (var parameter in parameters)
            {
                if (IsSimpleParameter(parameter, out Action<IEventFetcher, QueryParameter> action) || IsRegexParameter(parameter, out action))
                {
                    ApplyParameter(action, parameter);

                    if (parameter.Name == "maxEventCount")
                    {
                        maxEventCount = parameter.GetValue<int>()+1;
                    }
                }
                else
                {
                    throw new NotImplementedException($"Query parameter unexpected or not implemented: '{parameter.Name}'");
                }
            }

            var result = await _eventFetcher.Fetch(cancellationToken);

            if (maxEventCount.HasValue && result.Count() >= maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned more than the {maxEventCount} events allowed.");
            }
            else
            {
                return new PollResponse
                {
                    QueryName = Name,
                    EventList = result.ToArray()
                };
            }
        }

        private void ApplyParameter(Action<IEventFetcher, QueryParameter> simpleAction, QueryParameter parameter)
        {
            try
            {
                simpleAction(_eventFetcher, parameter);
            }
            catch(Exception ex)
            {
                throw new EpcisException(ExceptionType.QueryParameterException, ex.Message);
            }
        }

        private bool IsSimpleParameter(QueryParameter parameter, out Action<IEventFetcher, QueryParameter> action)
        {
            return SimpleParameters.TryGetValue(parameter.Name, out action);
        }

        private bool IsRegexParameter(QueryParameter parameter, out Action<IEventFetcher, QueryParameter> action)
        {
            var matchingRegex = RegexParameters.FirstOrDefault(x => Regex.Match(parameter.Name, x.Key, RegexOptions.Singleline).Success);
            action = matchingRegex.Value;

            return matchingRegex.Key != default;
        }
    }
}
