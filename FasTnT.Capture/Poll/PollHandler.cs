using FasTnT.Commands.Requests;
using FasTnT.Commands.Responses;
using FasTnT.Handlers.Poll.Queries;
using FasTnT.Model.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Handlers.Poll
{
    public class PollHandler : IRequestHandler<PollRequest, IEpcisResponse>
    {
        public async Task<IEpcisResponse> Handle(PollRequest request, CancellationToken cancellationToken)
        {
            switch (request.QueryName)
            {
                case "SimpleEventQuery":
                    return await SimpleEventQuery.Handle(request.Parameters);
                case "SimpleMasterdataQuery":
                    return await SimpleMasterdataQuery.Handle(request.Parameters);
                default:
                    throw new EpcisException(ExceptionType.NoSuchNameException, $"Unknown query: '{request.QueryName}'");
            }
        }
    }
}
