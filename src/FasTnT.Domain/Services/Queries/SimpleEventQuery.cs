using FasTnT.Domain;
using FasTnT.Domain.Services.Handlers.PredefinedQueries;
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
            // TODO: throw exception if some parameter name/values are not valid.
        }

        public async Task<IEnumerable<EpcisEvent>> Execute(IEnumerable<QueryParameter> parameters, IEventRepository query)
        {
            foreach(var parameter in parameters)
            {
                if (Equals(parameter.Name, "EQ_action")) query.WhereActionIn(parameter.Values.Select(Enumeration.GetByDisplayName<EventAction>).ToArray());
                else if (Equals(parameter.Name, "eventCountLimit")) query.SetEventLimit(parameter.GetValue<int>());
                else if (Equals(parameter.Name, "EQ_bizLocation")) query.WhereBusinessLocationIn(parameter.Values);
                else if (Equals(parameter.Name, "eventCountLimit")) query.SetEventLimit(parameter.GetValue<int>());
                else if (Equals(parameter.Name, "EQ_bizStep")) query.WhereBusinessStepIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_disposition")) query.WhereDispositionIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_eventID")) query.WhereEventIdIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_transformationID")) query.WhereTransformationIdIn(parameter.Values);
                else if (Equals(parameter.Name, "EXISTS_errorDeclaration")) query.WhereExistsErrorDeclaration();
                else if (Equals(parameter.Name, "EQ_errorReason")) query.WhereErrorReasonIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_correctiveEventID")) query.WhereCorrectiveEventIdIn(parameter.Values);
                else if (Equals(parameter.Name, "MATCH_anyEPC")) query.WhereEpcMatches(parameter.Values);

                // Regex-based parameters
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_eventTime")) ApplyEventTimeParameter(parameter, query);
                else if (Regex.IsMatch(parameter.Name, "^(GE|LT)_recordTime")) ApplyRecordTimeParameter(parameter, query);
                else if (Regex.IsMatch(parameter.Name, "^MATCH_")) ApplyEpcMatchParameter(parameter, query);
            }

            return await query.ToList();
        }

        private void ApplyEventTimeParameter(QueryParameter parameter, IEventRepository query)
        {
            var filterOperator = Enumeration.GetByDisplayName<FilterOperator>(parameter.Name.Substring(0, 2));
            query.WhereCaptureTimeMatches(filterOperator, parameter.GetValue<DateTime>());
        }

        private void ApplyRecordTimeParameter(QueryParameter parameter, IEventRepository query)
        {
            var filterOperator = Enumeration.GetByDisplayName<FilterOperator>(parameter.Name.Substring(0, 2));
            query.WhereRecordTimeMatches(filterOperator, parameter.GetValue<DateTime>());
        }

        private void ApplyEpcMatchParameter(QueryParameter parameter, IEventRepository repository)
        {
            var epcType = Enumeration.GetByDisplayName<EpcType>(parameter.Name.Substring(6));

            repository.WhereEpcMatches(parameter.Values, epcType);
        }
    }
}
