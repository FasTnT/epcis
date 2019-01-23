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
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public class SimpleEventQuery : IEpcisQuery
    {
        public string Name => "SimpleEventQuery";
        public bool AllowSubscription => true;

        private EpcisField _orderField = EpcisField.CaptureTime;
        private OrderDirection _orderDirection = OrderDirection.Descending;

        public void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false)
        {
            if(parameters.Any(x => x.Name == "maxEventCount") && parameters.Any(x => x.Name == "eventCountLimit"))
            {
                throw new EpcisException(ExceptionType.QueryParameterException, "maxEventCount and eventCountLimit parameters are mutually exclusive.");
            }
        }

        public async Task<IEnumerable<IEntity>> Execute(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork)
        {
            parameters = parameters ?? new QueryParameter[0];

            foreach (var parameter in parameters)
            {
                if (Equals(parameter.Name, "eventType")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.EventType, parameter.Values.Select(Enumeration.GetByDisplayName<EventType>).ToArray());
                else if (Equals(parameter.Name, "eventCountLimit")) unitOfWork.EventManager.SetLimit(parameter.GetValue<int>());
                else if (Equals(parameter.Name, "maxEventCount")) unitOfWork.EventManager.SetLimit(parameter.GetValue<int>() + 1);
                else if (Equals(parameter.Name, "EQ_action")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.Action, parameter.Values.Select(Enumeration.GetByDisplayName<EventAction>).ToArray());
                else if (Equals(parameter.Name, "EQ_bizLocation")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.BusinessLocation, parameter.Values);
                else if (Equals(parameter.Name, "EQ_bizStep")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.BusinessStep, parameter.Values);
                else if (Equals(parameter.Name, "EQ_disposition")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.Disposition, parameter.Values);
                else if (Equals(parameter.Name, "EQ_eventID")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.EventId, parameter.Values);
                else if (Equals(parameter.Name, "EQ_transformationID")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.TransformationId, parameter.Values);
                else if (Equals(parameter.Name, "EQ_readPoint")) unitOfWork.EventManager.WhereSimpleFieldIn(EpcisField.ReadPoint, parameter.Values);
                else if (Equals(parameter.Name, "EXISTS_errorDeclaration")) unitOfWork.EventManager.WhereExistsErrorDeclaration();
                else if (Equals(parameter.Name, "EQ_errorReason")) unitOfWork.EventManager.WhereErrorReasonIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_correctiveEventID")) unitOfWork.EventManager.WhereCorrectiveEventIdIn(parameter.Values);
                else if (Equals(parameter.Name, "WD_readPoint")) unitOfWork.EventManager.WhereMasterDataHierarchyContains(EpcisField.ReadPoint, parameter.Values);
                else if (Equals(parameter.Name, "WD_bizLocation")) unitOfWork.EventManager.WhereMasterDataHierarchyContains(EpcisField.BusinessLocation, parameter.Values);

                else if (Equals(parameter.Name, "orderBy")) _orderField = Enumeration.GetByDisplayName<EpcisField>(parameter.Values.Single());
                else if (Equals(parameter.Name, "orderDirection")) _orderDirection = Enumeration.GetByDisplayName<OrderDirection>(parameter.Values.Single());

                // Family of parameters (regex name)
                else if (Regex.IsMatch(parameter.Name, "^EQ_(source|destination)_")) ApplySourceDestinationParameter(parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^EQ_bizTransaction_")) ApplyBusinessTransactionParameter(parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_eventTime")) ApplyTimeParameter(EpcisField.CaptureTime, parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_recordTime")) ApplyTimeParameter(EpcisField.RecordTime, parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^MATCH_")) ApplyEpcMatchParameter(parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_quantity$")) ApplyQuantityParameter(parameter, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_INNER_ILMD_")) ApplyCustomFieldParameter(parameter, true, FieldType.Ilmd, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_ILMD_")) ApplyCustomFieldParameter(parameter, false, FieldType.Ilmd, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_INNER_ILMD_")) ApplyExistCustomFieldParameter(parameter, true, FieldType.Ilmd, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_ILMD_")) ApplyExistCustomFieldParameter(parameter, false, FieldType.Ilmd, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_INNER_")) ApplyCustomFieldParameter(parameter, true, FieldType.EventExtension, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_")) ApplyCustomFieldParameter(parameter, false, FieldType.EventExtension, unitOfWork);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_INNER")) ApplyExistCustomFieldParameter(parameter, true, FieldType.EventExtension, unitOfWork);
                // TODO: masterdata attributes parameters
                //else if (Regex.IsMatch(parameter.Name, "^EXISTS_")) ApplyExistAttributeParameter(parameter, false, FieldType.EventExtension, unitOfWork);
                //else if (Regex.IsMatch(parameter.Name, "^HASATTR_")) ApplyHasAttributeParameter(parameter, false, FieldType.EventExtension, unitOfWork);

                else throw new NotImplementedException($"Query parameter unexpected or not implemented: '{parameter.Name}'");
            }

            // Set order by filter
            unitOfWork.EventManager.OrderBy(_orderField, _orderDirection);
            var results = await unitOfWork.EventManager.ToList();

            // Check for the maxEventCount parameter
            if(parameters.Any(x => x.Name == "maxEventCount") && results.Count() == parameters.Last(x => x.Name == "maxEventCount").GetValue<int>() + 1)
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");

            return results;
        }

        private void ApplyQuantityParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            if (parameter.Values.Length > 1) throw new EpcisException(ExceptionType.QueryParameterException, "QuantityParameter must have only one value");

            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            unitOfWork.EventManager.WhereEpcQuantityMatches(filterOperator, parameter.GetValue<double>());
        }

        private void ApplyBusinessTransactionParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var txName = parameter.Name.Split('_', 3)[2];
            unitOfWork.EventManager.WhereBusinessTransactionValueIn(txName, parameter.Values);
        }

        private void ApplySourceDestinationParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var name = parameter.Name.Split('_', 3)[2];
            var type = parameter.Name.StartsWith("EQ_source") ? SourceDestinationType.Source : SourceDestinationType.Destination;

            unitOfWork.EventManager.WhereSourceDestinationValueIn(name, type, parameter.Values);
        }

        private void ApplyExistCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IUnitOfWork unitOfWork)
        {
            var parts = parameter.Name.Split('_', 4);
            unitOfWork.EventManager.WhereCustomFieldExists(inner, fieldType, parts[2], parts[3]);
        }

        private void ApplyCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IUnitOfWork unitOfWork)
        {
            var parts = parameter.Name.Split('_', 4);

            if (parameter.Values.Length > 1)
            {
                if (!parameter.Name.StartsWith("EQ_")) throw new EpcisException(ExceptionType.QueryParameterException, "Custom Field parameter must be 'EQ' if multiple values are present.");
                unitOfWork.EventManager.WhereCustomFieldMatches(inner, fieldType, parts[2], parts[3], parameter.Values);
            }
            else
            {
                var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
                unitOfWork.EventManager.WhereCustomFieldMatches(inner, fieldType, parts[2], parts[3], filterOperator, parameter.GetSingleValue());
            }
        }

        private void ApplyTimeParameter(EpcisField field, QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            unitOfWork.EventManager.WhereSimpleFieldMatches(field, filterOperator, parameter.GetValue<DateTime>());
        }

        private void ApplyEpcMatchParameter(QueryParameter parameter, IUnitOfWork unitOfWork)
        {
            unitOfWork.EventManager.WhereEpcMatches(parameter.Values.Select(x => x.Replace("*", "%")).ToArray(), parameter.GetMatchEpcTypes());
        }
    }
}
