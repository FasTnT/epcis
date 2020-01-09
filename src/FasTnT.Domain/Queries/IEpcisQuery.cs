using FasTnT.Commands.Responses;
using FasTnT.Model.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Queries
{
    public interface IEpcisQuery
    {
        string Name { get; }
        Task<IEpcisResponse> Handle(QueryParameter[] parameters, CancellationToken cancellationToken);
    }
}
