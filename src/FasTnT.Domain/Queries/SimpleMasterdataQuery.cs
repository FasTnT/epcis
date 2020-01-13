using FasTnT.Commands.Responses;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public class SimpleMasterdataQuery : IEpcisQuery
    {
        public string Name => "SimpleMasterdataQuery";
        public bool AllowSubscription => false;

        public Task<IEpcisResponse> Handle(QueryParameter[] parameters, CancellationToken cancellationToken)
        {
            throw new EpcisException(ExceptionType.ImplementationException, "SimpleEventQuery is not implemented yet");
        }
    }
}
