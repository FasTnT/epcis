using FasTnT.Domain.Persistence;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Extensions;
using FasTnT.Model.Queries.Enums;
using FasTnT.Model.Responses;
using FasTnT.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public class SimpleEventQuery : IEpcisQuery
    {
        private static readonly string[] _specificNames = new[] { "eventType", "orderBy", "orderDirection" };
        private static readonly string[] _anyValuePrefixes = new[] { "EQ_", "EXISTS_", "EQATTR_", "HASATTR_", "WD_", "MATCH_" };
        private static readonly string[] _comparisonPrefixes = new[] { "GE_", "LE_", "GT_", "LT_" };

        private static IDictionary<string, Action<IUnitOfWork, QueryParameter>> SimpleParameters = new Dictionary<string, Action<IUnitOfWork, QueryParameter>>
        {
            { "eventType",               (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.EventType, param.Values.Select(Enumeration.GetByDisplayName<EventType>).ToArray()) },
            { "eventCountLimit",         (uow, param) => uow.EventManager.SetLimit(param.GetValue<int>()) },
            { "maxEventCount",           (uow, param) => uow.EventManager.SetLimit(param.GetValue<int>() + 1) },
            { "EQ_action",               (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.Action, param.Values.Select(Enumeration.GetByDisplayName<EventAction>).ToArray()) },
            { "EQ_bizLocation",          (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.BusinessLocation, param.Values) },
            { "EQ_bizStep",              (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.BusinessStep, param.Values) },
            { "EQ_disposition",          (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.Disposition, param.Values) },
            { "EQ_eventID",              (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.EventId, param.Values) },
            { "EQ_transformationID",     (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.TransformationId, param.Values) },
            { "EQ_readPoint",            (uow, param) => uow.EventManager.WhereSimpleFieldIn(EpcisField.ReadPoint, param.Values) },
            { "EXISTS_errorDeclaration", (uow, param) => uow.EventManager.WhereExistsErrorDeclaration() },
            { "EQ_errorReason",          (uow, param) => uow.EventManager.WhereErrorReasonIn(param.Values) },
            { "EQ_correctiveEventID",    (uow, param) => uow.EventManager.WhereCorrectiveEventIdIn(param.Values) },
            { "WD_readPoint",            (uow, param) => uow.EventManager.WhereMasterDataHierarchyContains(EpcisField.ReadPoint, param.Values) },
            { "WD_bizLocation",          (uow, param) => uow.EventManager.WhereMasterDataHierarchyContains(EpcisField.BusinessLocation, param.Values) }
        };

        private static IDictionary<string, Action<IUnitOfWork, QueryParameter>> RegexParameters = new Dictionary<string, Action<IUnitOfWork, QueryParameter>>
        {
            { "^EQ_(source|destination)_",     (uow, param) => ApplySourceDestinationParameter(param, uow) },
            { "^EQ_bizTransaction_",           (uow, param) => ApplyBusinessTransactionParameter(param, uow) },
            { "^(GE|LT)_eventTime",            (uow, param) => ApplyTimeParameter(EpcisField.CaptureTime, param, uow) },
            { "^(GE|LT)_recordTime",           (uow, param) => ApplyTimeParameter(EpcisField.RecordTime, param, uow) },
            { "^MATCH_",                       (uow, param) => ApplyEpcMatchParameter(param, uow) },
            { "^(EQ|GT|LT|GE|LE)_quantity$",   (uow, param) => ApplyQuantityParameter(param, uow) },
            { "^(EQ|GT|LT|GE|LE)_INNER_ILMD_", (uow, param) => ApplyCustomFieldParameter(param, true, FieldType.Ilmd, uow) },
            { "^(EQ|GT|LT|GE|LE)_ILMD_",       (uow, param) => ApplyCustomFieldParameter(param, false, FieldType.Ilmd, uow) },
            { "^EXISTS_INNER_ILMD_",           (uow, param) => ApplyExistCustomFieldParameter(param, true, FieldType.Ilmd, uow) },
            { "^EXISTS_ILMD_",                 (uow, param) => ApplyExistCustomFieldParameter(param, false, FieldType.Ilmd, uow) },
            { "^(EQ|GT|LT|GE|LE)_INNER_",      (uow, param) => ApplyCustomFieldParameter(param, true, FieldType.EventExtension, uow) },
            { "^(EQ|GT|LT|GE|LE)_",            (uow, param) => ApplyCustomFieldParameter(param, false, FieldType.EventExtension, uow) },
            { "^EXISTS_INNER",                 (uow, param) => ApplyExistCustomFieldParameter(param, true, FieldType.EventExtension, uow) },
            { "^EQATTR_",                      (uow, param) => ApplyExistAttributeParameter(param, uow) },
            { "^HASATTR_",                     (uow, param) => ApplyHasAttributeParameter(param, uow) }
        };

        public string Name => "SimpleEventQuery";
        public bool AllowSubscription => true;

        private EpcisField _orderField = EpcisField.CaptureTime;
        private OrderDirection _orderDirection = OrderDirection.Descending;

        public void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false)
        {
            parameters = parameters ?? new QueryParameter[0];

            foreach (var parameter in parameters)
            {
                if (_specificNames.Contains(parameter.Name) || _anyValuePrefixes.Any(p => parameter.Name.StartsWith(p))) continue;
                if (_comparisonPrefixes.Any(p => parameter.Name.StartsWith(p) && parameter.ContainsSingleValueOfType(new[] { typeof(DateTime), typeof(double) }))) continue;
                if (!subscription && new[] { "maxEventCount", "eventCountLimit" }.Contains(parameter.Name) && parameter.ContainsSingleValueOfType(typeof(double))) continue;

                throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter '{parameter.Name}' is unknown, has invalid value or not allowed in this context.");
            }

            if (parameters.Any(x => x.Name == "maxEventCount") && parameters.Any(x => x.Name == "eventCountLimit"))
            {
                throw new EpcisException(ExceptionType.QueryParameterException, "maxEventCount and eventCountLimit parameters are mutually exclusive.");
            }
        }

        public async Task<IEnumerable<IEntity>> Execute(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            parameters = parameters ?? new QueryParameter[0];

            foreach (var parameter in parameters)
            {
                if (IsOrderParameter(parameter))
                {
                    HandleOrderParameter(parameter);
                }
                else if (IsSimpleParameter(parameter, out Action<IUnitOfWork, QueryParameter> simpleAction))
                {
                    simpleAction(unitOfWork, parameter);
                }
                else if (IsRegexParameter(parameter, out Action<IUnitOfWork, QueryParameter> regexAction))
                {
                    regexAction(unitOfWork, parameter);
                }
                else
                {
                    throw new NotImplementedException($"Query parameter unexpected or not implemented: '{parameter.Name}'");
                }
            }

            return await FetchResults(parameters, unitOfWork, cancellationToken);
        }

        private bool IsOrderParameter(QueryParameter parameter)
        {
            return Equals(parameter.Name, "orderBy") || Equals(parameter.Name, "orderDirection");
        }

        private bool IsSimpleParameter(QueryParameter parameter, out Action<IUnitOfWork, QueryParameter> action)
        {
            return SimpleParameters.TryGetValue(parameter.Name, out action);
        }

        private bool IsRegexParameter(QueryParameter parameter, out Action<IUnitOfWork, QueryParameter> action)
        {
            var matchingRegex = RegexParameters.FirstOrDefault(x => Regex.Match(parameter.Name, x.Key, RegexOptions.Singleline).Success);

            action = matchingRegex.Value;

            return matchingRegex.Key != default;
        }

        private void HandleOrderParameter(QueryParameter parameter)
        {
            if (Equals(parameter.Name, "orderBy"))
            {
                _orderField = Enumeration.GetByDisplayName<EpcisField>(parameter.Values.Single());
            }
            else if (Equals(parameter.Name, "orderDirection"))
            {
                _orderDirection = Enumeration.GetByDisplayName<OrderDirection>(parameter.Values.Single());
            }
        }

        private async Task<IEnumerable<IEntity>> FetchResults(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            unitOfWork.EventManager.OrderBy(_orderField, _orderDirection); // Set order by filter

            var results = await unitOfWork.EventManager.ToList(cancellationToken);

            // Check for the maxEventCount parameter
            if (parameters.Any(x => x.Name == "maxEventCount") && results.Count() == parameters.Last(x => x.Name == "maxEventCount").GetValue<int>() + 1)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");
            }

            return results;
        }

        private static void ApplyQuantityParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            if (parameter.Values.Length > 1) throw new EpcisException(ExceptionType.QueryParameterException, "QuantityParameter must have only one value");

            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            unitOfWork.EventManager.WhereEpcQuantityMatches(filterOperator, parameter.GetValue<double>());
        }

        private static void ApplyBusinessTransactionParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var txName = parameter.Name.Split('_', 3)[2];
            unitOfWork.EventManager.WhereBusinessTransactionValueIn(txName, parameter.Values);
        }

        private static void ApplySourceDestinationParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var name = parameter.Name.Split('_', 3)[2];
            var type = parameter.Name.StartsWith("EQ_source") ? SourceDestinationType.Source : SourceDestinationType.Destination;

            unitOfWork.EventManager.WhereSourceDestinationValueIn(name, type, parameter.Values);
        }

        private static void ApplyExistCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IUnitOfWork unitOfWork)
        {
            var parts = parameter.Name.Split('_', 4);
            unitOfWork.EventManager.WhereCustomFieldExists(inner, fieldType, parts[2], parts[3]);
        }

        private static void ApplyCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IUnitOfWork unitOfWork)
        {
            var parts = parameter.Name.Split('_', 4);
            var field = new CustomField { Name = parts[3], Namespace = parts[2], Type = fieldType };

            if (parameter.Values.Length > 1)
            {
                if (!parameter.Name.StartsWith("EQ_")) throw new EpcisException(ExceptionType.QueryParameterException, "Custom Field parameter must be 'EQ' if multiple values are present.");
                unitOfWork.EventManager.WhereCustomFieldMatches(field, inner, parameter.Values);
            }
            else
            {
                var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
                unitOfWork.EventManager.WhereCustomFieldMatches(field, inner, filterOperator, parameter.GetSingleValue());
            }
        }

        private static void ApplyTimeParameter(EpcisField field, QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            unitOfWork.EventManager.WhereSimpleFieldMatches(field, filterOperator, parameter.GetValue<DateTime>());
        }

        private static void ApplyEpcMatchParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            unitOfWork.EventManager.WhereEpcMatches(parameter.Values.Select(x => x.Replace("*", "%")).ToArray(), parameter.GetMatchEpcTypes());
        }

        private static void ApplyExistAttributeParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var parts = parameter.Name.Substring(7).Split("_", 2);
            var attribute = Enumeration.GetByDisplayName<EpcisField>(parts[0]);
            unitOfWork.EventManager.WhereMasterdataAttributeValueIn(attribute, parts[1], parameter.Values);
        }

        private static void ApplyHasAttributeParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var attribute = Enumeration.GetByDisplayName<EpcisField>(parameter.Name.Substring(8));
            unitOfWork.EventManager.WhereMasterdataHasAttribute(attribute, parameter.Values);
        }
    }
}
