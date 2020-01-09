using FasTnT.Commands.Responses;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using System.Threading.Tasks;

namespace FasTnT.Domain.Handlers.Poll.Queries
{
    public static class SimpleMasterdataQuery
    {
        public static Task<IEpcisResponse> Handle(QueryParameter[] parameters)
        {
            throw new EpcisException(ExceptionType.ImplementationException, "SimpleEventQuery is not implemented yet");
        }
    }
}
