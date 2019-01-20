using FasTnT.Domain.Persistence;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using FasTnT.Model.Queries.Implementations;
using FasTnT.Model.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Domain.Services
{
    public class QueryService
    {
        private readonly IEpcisQuery[] _queries;
        private readonly IUnitOfWork _unitOfWork;

        public QueryService(IEpcisQuery[] queries, IUnitOfWork unitOfWork)
        {
            _queries = queries;
            _unitOfWork = unitOfWork;
        }

        public Task<GetQueryNamesResponse> Process(GetQueryNames query) => Task.Run(() => new GetQueryNamesResponse { QueryNames = _queries.Select(x => x.Name) });
        public Task<GetStandardVersionResponse> Process(GetStandardVersion query) => Task.Run(() => new GetStandardVersionResponse { Version = Constants.StandardVersion });
        public Task<GetVendorVersionResponse> Process(GetVendorVersion query) => Task.Run(() => new GetVendorVersionResponse { Version = Constants.ProductVersion });
        public async Task<GetSubscriptionIdsResult> Process(GetSubscriptionIds query) => new GetSubscriptionIdsResult { SubscriptionIds = (await _unitOfWork.SubscriptionManager.GetAll()).Where(s => s.QueryName == query.QueryName).Select(x => x.SubscriptionId) };

        public async Task<PollResponse> Process(Poll query)
        {
            var epcisQuery = _queries.SingleOrDefault(x => x.Name == query.QueryName);

            if (epcisQuery == null)
            {
                throw new Exception($"Unknown query: '{query.QueryName}'");
            }
            else
            {
                try
                {
                    epcisQuery.ValidateParameters(query.Parameters);

                    var results = await epcisQuery.Execute(query.Parameters, _unitOfWork);
                    return new PollResponse { QueryName = query.QueryName, Entities = results };
                }
                catch (Exception ex)
                {
                    throw new EpcisException(ExceptionType.QueryParameterException, ex.Message);
                }
            }
        }
    }
}
