using FasTnT.Domain.Commands;
using FasTnT.Model.Queries;

namespace FasTnT.Commands.Requests
{
    public class PollRequest : IQueryRequest
    {
        public string QueryName { get; set; }
        public QueryParameter[] Parameters { get; set; }
    }
}
