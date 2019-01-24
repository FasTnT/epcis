using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Extensions;
using FasTnT.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public class SimpleMasterDataQuery : IEpcisQuery
    {
        public string Name => "SimpleMasterDataQuery";
        public bool AllowSubscription => false;

        public void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false) { }

        public async Task<IEnumerable<IEntity>> Execute(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork)
        {
            var attributeNames = new List<string>();
            bool includeAttributes = false, includeChilren = false;

            foreach (var parameter in parameters)
            {
                if (Equals(parameter.Name, "includeAttributes")) includeAttributes = parameter.GetValue<bool>();
                else if (Equals(parameter.Name, "vocabularyName")) unitOfWork.MasterDataManager.WhereTypeIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_name")) unitOfWork.MasterDataManager.WhereIdIn(parameter.Values);
                else if (Equals(parameter.Name, "HASATTR")) unitOfWork.MasterDataManager.WhereAnyAttributeNamed(parameter.Values);
                else if (Equals(parameter.Name, "maxElementCount")) unitOfWork.MasterDataManager.Limit(parameter.GetValue<int>() + 1);
                else if (Equals(parameter.Name, "WD_name")) unitOfWork.MasterDataManager.WhereIsDescendantOf(parameter.Values);
                else if (Equals(parameter.Name, "includeChildren")) includeChilren = parameter.GetValue<bool>();
                else if (Equals(parameter.Name, "attributeNames")) attributeNames.AddRange(parameter.Values);
                else if (Regex.IsMatch(parameter.Name, "^EQATTR_")) throw new NotImplementedException("Parameter 'EQATTR_*' is not implemented yet.");
            }

            var results = await unitOfWork.MasterDataManager.ToList(includeAttributes ? attributeNames.ToArray() : null, includeChilren);

            // Check for the maxElementCount parameter
            if (parameters.Any(x => x.Name == "maxElementCount") && results.Count() == parameters.Last(x => x.Name == "maxElementCount").GetValue<int>() + 1)
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");

            return results;
        }
    }
}
