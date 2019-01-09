using FasTnT.Domain.Services.Handlers.PredefinedQueries;
using FasTnT.Model.Events.Enums;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Extensions;
using FasTnT.Model.Queries.Enums;
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

        public void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false)
        {
            if(parameters.Any(x => x.Name == "maxEventCount") && parameters.Any(x => x.Name == "eventCountLimit"))
            {
                throw new EpcisException(ExceptionType.QueryParameterException, "maxEventCount and eventCountLimit parameters are mutually exclusive.");
            }
        }

        public async Task<IEnumerable<EpcisEvent>> Execute(IEnumerable<QueryParameter> parameters, IEventRepository repository)
        {
            foreach(var parameter in parameters)
            {
                if (Equals(parameter.Name, "eventType")) repository.WhereSimpleFieldIn(EpcisField.EventType, parameter.Values.Select(Enumeration.GetByDisplayName<EventType>).ToArray());
                else if (Equals(parameter.Name, "EQ_action")) repository.WhereSimpleFieldIn(EpcisField.Action, parameter.Values.Select(Enumeration.GetByDisplayName<EventAction>).ToArray());
                else if (Equals(parameter.Name, "eventCountLimit")) repository.SetLimit(parameter.GetValue<int>());
                else if (Equals(parameter.Name, "maxEventCount")) repository.SetLimit(parameter.GetValue<int>() + 1);
                else if (Equals(parameter.Name, "EQ_bizLocation")) repository.WhereSimpleFieldIn(EpcisField.BusinessLocation, parameter.Values);
                else if (Equals(parameter.Name, "EQ_bizStep")) repository.WhereSimpleFieldIn(EpcisField.BusinessStep, parameter.Values);
                else if (Equals(parameter.Name, "EQ_disposition")) repository.WhereSimpleFieldIn(EpcisField.Disposition, parameter.Values);
                else if (Equals(parameter.Name, "EQ_eventID")) repository.WhereSimpleFieldIn(EpcisField.EventId, parameter.Values);
                else if (Equals(parameter.Name, "EQ_transformationID")) repository.WhereSimpleFieldIn(EpcisField.TransformationId, parameter.Values);
                else if (Equals(parameter.Name, "EXISTS_errorDeclaration")) repository.WhereExistsErrorDeclaration();
                else if (Equals(parameter.Name, "EQ_errorReason")) repository.WhereErrorReasonIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_correctiveEventID")) repository.WhereCorrectiveEventIdIn(parameter.Values);
                else if (Equals(parameter.Name, "MATCH_anyEPC")) repository.WhereEpcMatches(parameter.Values);

                // Regex-based parameters
                else if (Regex.IsMatch(parameter.Name, "^EQ_(source|destination)_")) ApplySourceDestinationParameter(parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^EQ_bizTransaction_")) ApplyBusinessTransactionParameter(parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_eventTime")) ApplyTimeParameter(EpcisField.CaptureTime, parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_recordTime")) ApplyTimeParameter(EpcisField.RecordTime, parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^MATCH_")) ApplyEpcMatchParameter(parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_quantity$")) ApplyQuantityParameter(parameter, repository);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_INNER_ILMD_")) ApplyCustomFieldParameter(parameter, true, FieldType.Ilmd, repository);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_ILMD_")) ApplyCustomFieldParameter(parameter, false, FieldType.Ilmd, repository);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_INNER_ILMD_")) ApplyExistCustomFieldParameter(parameter, true, FieldType.Ilmd, repository);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_ILMD_")) ApplyExistCustomFieldParameter(parameter, false, FieldType.Ilmd, repository);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_INNER_")) ApplyCustomFieldParameter(parameter, true, FieldType.EventExtension, repository);
                else if (Regex.IsMatch(parameter.Name, "^(EQ|GT|LT|GE|LE)_")) ApplyCustomFieldParameter(parameter, false, FieldType.EventExtension, repository);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_INNER")) ApplyExistCustomFieldParameter(parameter, true, FieldType.EventExtension, repository);
                else if (Regex.IsMatch(parameter.Name, "^EXISTS_")) ApplyExistCustomFieldParameter(parameter, false, FieldType.EventExtension, repository);

                //TODO: add missing parameters: ATTR-based, WD_*
                else throw new NotImplementedException($"Query parameter unexpected or not implemented: '{parameter.Name}'");
            }

            var results = await repository.ToList();

            // Check for the maxEventCount parameter
            if(parameters.Any(x => x.Name == "maxEventCount") && results.Count() == parameters.Last(x => x.Name == "maxEventCount").GetValue<int>() + 1)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");
            }

            return results;
        }

        private void ApplyQuantityParameter(QueryParameter parameter, IEventRepository repository)
        {
            if (parameter.Values.Length > 1) throw new EpcisException(ExceptionType.QueryParameterException, "QuantityParameter must have only one value");

            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            repository.WhereEpcQuantityMatches(filterOperator, parameter.GetValue<double>());
        }

        private void ApplyBusinessTransactionParameter(QueryParameter parameter, IEventRepository repository)
        {
            var txName = parameter.Name.Split('_', 3)[2];
            repository.WhereBusinessTransactionValueIn(txName, parameter.Values);
        }

        private void ApplySourceDestinationParameter(QueryParameter parameter, IEventRepository repository)
        {
            var name = parameter.Name.Split('_', 3)[2];
            var type = parameter.Name.StartsWith("EQ_source") ? SourceDestinationType.Source : SourceDestinationType.Destination;

            repository.WhereSourceDestinationValueIn(name, type, parameter.Values);
        }

        private void ApplyExistCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IEventRepository repository)
        {
            var parts = parameter.Name.Split('_', 4);
            repository.WhereCustomFieldExists(inner, fieldType, parts[2], parts[3]);
        }

        private void ApplyCustomFieldParameter(QueryParameter parameter, bool inner, FieldType fieldType, IEventRepository repository)
        {
            var parts = parameter.Name.Split('_', 4);

            if (parameter.Values.Length > 1)
            {
                if (!parameter.Name.StartsWith("EQ_")) throw new EpcisException(ExceptionType.QueryParameterException, "Custom Field parameter must be 'EQ' if multiple values are present.");

                repository.WhereCustomFieldMatches(inner, fieldType, parts[2], parts[3], parameter.Values);
            }
            else
            {
                var valueType = "";
                var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
                repository.WhereCustomFieldMatches(inner, fieldType, parts[2], parts[3], filterOperator, parameter.GetValue<double>());
            }
        }

        private void ApplyTimeParameter(EpcisField field, QueryParameter parameter, IEventRepository repository)
        {
            var filterOperator = Enumeration.GetByDisplayName<FilterComparator>(parameter.Name.Substring(0, 2));
            repository.WhereSimpleFieldMatches(field, filterOperator, parameter.GetValue<DateTime>());
        }

        //TODO: fix epcType parsing: does not comply with EPCIS 1.2 specification
        private void ApplyEpcMatchParameter(QueryParameter parameter, IEventRepository repository)
        {
            var epcType = Enumeration.GetByDisplayName<EpcType>(parameter.Name.Substring(6));
            repository.WhereEpcMatches(parameter.Values, epcType);
        }
    }
}
