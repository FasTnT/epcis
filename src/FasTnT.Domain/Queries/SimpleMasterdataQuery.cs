using FasTnT.Commands.Responses;
using FasTnT.Domain.Data;
using FasTnT.Domain.Data.Model.Filters;
using FasTnT.Domain.Utils;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public class SimpleMasterdataQuery : IEpcisQuery
    {
        public string Name => "SimpleMasterDataQuery";
        public bool AllowSubscription => false;
        private List<string> _attributeNames = new List<string>();
        private bool _includeAttributes, _includeChildren;

        private static IDictionary<string, Action<IMasterdataFetcher, QueryParameter>> SimpleParameters = new Dictionary<string, Action<IMasterdataFetcher, QueryParameter>>
        {
            { "vocabularyName",    (fetcher, param) => fetcher.Apply(new MasterdataTypeFilter { Values = param.Values }) },
            { "EQ_name",           (fetcher, param) => fetcher.Apply(new MasterdataNameFilter { Values = param.Values }) },
            { "HASATTR",           (fetcher, param) => fetcher.Apply(new MasterdataExistsAttibuteFilter { Values = param.Values }) },
            { "maxElementCount",   (fetcher, param) => fetcher.Apply(new LimitFilter { Value = param.GetValue<int>() +1 }) },
            { "WD_name",           (fetcher, param) => fetcher.Apply(new MasterdataDescendentNameFilter { Values = param.Values }) }
        };

        private readonly IMasterdataFetcher _masterdataFetcher;

        public SimpleMasterdataQuery(IMasterdataFetcher masterdataFetcher)
        {
            _masterdataFetcher = masterdataFetcher;
        }

        public async Task<PollResponse> Handle(QueryParameter[] parameters, CancellationToken cancellationToken)
        {
            foreach (var parameter in parameters)
            {
                if (IsAttributeParameter(parameter))
                {
                    HandleAttributeParameter(parameter);
                }
                else if (SimpleParameters.TryGetValue(parameter.Name, out Action<IMasterdataFetcher, QueryParameter> action))
                {
                    action(_masterdataFetcher, parameter);
                }
                else
                {
                    throw new NotImplementedException($"Parameter '{parameter.Name}' is not implemented yet.");
                }
            }

            return await FetchResults(parameters, cancellationToken);
        }

        private bool IsAttributeParameter(QueryParameter parameter) => Equals(parameter.Name, "includeAttributes") || Equals(parameter.Name, "attributeNames") || Equals(parameter.Name, "includeChildren");

        private void HandleAttributeParameter(QueryParameter parameter)
        {
            if (Equals(parameter.Name, "includeAttributes"))
            {
                _includeAttributes = parameter.GetValue<bool>();
            }
            else if (Equals(parameter.Name, "attributeNames"))
            {
                _attributeNames.AddRange(parameter.Values);
            }
            else if (Equals(parameter.Name, "includeChildren"))
            {
                _includeChildren = parameter.GetValue<bool>();
            }
        }

        private async Task<PollResponse> FetchResults(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var results = await _masterdataFetcher.Fetch(_includeAttributes ? _attributeNames.ToArray() : null, _includeChildren, cancellationToken);

            // Check for the maxElementCount parameter
            if (parameters.Any(x => x.Name == "maxElementCount") && results.Count() == parameters.Last(x => x.Name == "maxElementCount").GetValue<int>() + 1)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");
            }

            return new PollResponse
            {
                MasterdataList = results.ToArray(),
                QueryName = Name
            };
        }
    }
}
