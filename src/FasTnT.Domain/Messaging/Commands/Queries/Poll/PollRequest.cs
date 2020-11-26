using FasTnT.Domain.Commands;
using FasTnT.Model.Queries;
using System.Collections.Generic;

namespace FasTnT.Commands.Requests
{
    public class PollRequest : IQueryRequest
    {
        public string QueryName { get; set; }
        public IEnumerable<QueryParameter> Parameters { get; set; }
    }
}
