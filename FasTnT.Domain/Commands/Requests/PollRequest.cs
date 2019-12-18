using FasTnT.Commands.Responses;
using FasTnT.Model.Queries;
using MediatR;

namespace FasTnT.Commands.Requests
{
    public class PollRequest : IRequest<IEpcisResponse>
    {
        public string QueryName { get; set; }
        public QueryParameter[] Parameters { get; set; }
    }
}
