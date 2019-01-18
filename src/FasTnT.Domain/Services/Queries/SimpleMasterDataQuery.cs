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
            var includeAttributes = false;
            foreach(var parameter in parameters)
            {
                if (Equals(parameter.Name, "includeAttributes")) includeAttributes = parameter.GetValue<bool>();
                else if (Equals(parameter.Name, "vocabularyName")) unitOfWork.MasterDataManager.WhereTypeIn(parameter.Values);
                else if (Equals(parameter.Name, "EQ_name")) unitOfWork.MasterDataManager.WhereIdIn(parameter.Values);
                else if (Equals(parameter.Name, "HASATTR")) unitOfWork.MasterDataManager.WhereAnyAttributeNamed(parameter.Values);
                else if (Equals(parameter.Name, "maxElementCount")) unitOfWork.MasterDataManager.Limit(parameter.GetValue<int>() + 1);
                else if (Equals(parameter.Name, "WD_name")) throw new NotImplementedException("Parameter 'includeChildren' is not implemented yet.");
                else if (Equals(parameter.Name, "includeChildren")) throw new NotImplementedException("Parameter 'includeChildren' is not implemented yet.");
                else if (Equals(parameter.Name, "attributeNames")) throw new NotImplementedException("Parameter 'attributeNames' is not implemented yet.");
                else if (Regex.IsMatch(parameter.Name, "^EQATTR_")) throw new NotImplementedException("Parameter 'EQATTR_*' is not implemented yet.");
            }

            var results = await unitOfWork.MasterDataManager.ToList(includeAttributes);

            // Check for the maxEventCount parameter
            if (parameters.Any(x => x.Name == "maxEventCount") && results.Count() == parameters.Last(x => x.Name == "maxEventCount").GetValue<int>() + 1)
                throw new EpcisException(ExceptionType.QueryTooLargeException, "Too many results returned by the request");

            return results;
        }
    }
}
