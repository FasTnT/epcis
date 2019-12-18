using FasTnT.Commands.Responses;
using FasTnT.Model.Exceptions;
using FasTnT.Model.Queries;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Poll.Queries
{
    public class SimpleEventQuery
    {
        public static async Task<IEpcisResponse> Handle(QueryParameter[] parameters)
        {
            throw new EpcisException(ExceptionType.ImplementationException, "SimpleEventQuery is not implemented yet");
        }
    }
}
