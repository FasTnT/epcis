using FasTnT.Commands.Responses;
using FasTnT.Model.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public interface IEpcisQuery
    {
        string Name { get; }
        bool AllowSubscription { get; }

        Task<PollResponse> Handle(QueryParameter[] parameters, CancellationToken cancellationToken);
    }
}
