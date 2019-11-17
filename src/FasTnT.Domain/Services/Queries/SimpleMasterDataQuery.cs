using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Extensions;
using FasTnT.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public class SimpleMasterDataQuery : IEpcisQuery
    {
        public string Name => "SimpleMasterDataQuery";
        public bool AllowSubscription => false;

        private List<string> _attributeNames = new List<string>();
        private bool _includeAttributes, _includeChildren;

        private static IDictionary<string, Action<IUnitOfWork, QueryParameter>> SimpleParameters = new Dictionary<string, Action<IUnitOfWork, QueryParameter>>
        {
            { "vocabularyName",    (uow, param) => uow.MasterDataManager.WhereTypeIn(param.Values) },
            { "EQ_name",           (uow, param) => uow.MasterDataManager.WhereIdIn(param.Values) },
            { "HASATTR",           (uow, param) => uow.MasterDataManager.WhereAnyAttributeNamed(param.Values) },
            { "maxElementCount",   (uow, param) => uow.MasterDataManager.Limit(param.GetValue<int>() + 1) },
            { "WD_name",           (uow, param) => uow.MasterDataManager.WhereIsDescendantOf(param.Values) }
        };

        public void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false) { }

        public async Task<IEnumerable<IEntity>> Execute(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            foreach (var parameter in parameters)
            {
                if (IsAttributeParameter(parameter))
                {
                    HandleAttributeParameter(parameter);
                }
                else if (SimpleParameters.TryGetValue(parameter.Name, out Action<IUnitOfWork, QueryParameter> action))
                {
                    action(unitOfWork, parameter);
                }
                else
                {
                    throw new NotImplementedException($"Parameter '{parameter.Name}' is not implemented yet.");
                }
            }

            return await FetchResults(parameters, unitOfWork, cancellationToken);
        }

        private bool IsAttributeParameter(QueryParameter parameter)
        {
            return Equals(parameter.Name, "includeAttributes") || Equals(parameter.Name, "attributeNames") || Equals(parameter.Name, "includeChildren");
        }

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

        private async Task<IEnumerable<IEntity>> FetchResults(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            var results = await unitOfWork.MasterDataManager.ToList(_includeAttributes ? _attributeNames.ToArray() : null, _includeChildren, cancellationToken);

            // Check for the maxElementCount parameter
            if (parameters.Any(x => x.Name == "maxElementCount") && results.Count() == parameters.Last(x => x.Name == "maxElementCount").GetValue<int>() + 1)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");
            }

            return results;
        }
    }
}
