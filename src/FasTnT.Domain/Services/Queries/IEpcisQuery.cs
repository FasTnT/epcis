using FasTnT.Domain.Persistence;
using FasTnT.Model.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FasTnT.Model.Queries.Implementations
{
    public interface IEpcisQuery
    {
        string Name { get; }
        bool AllowSubscription { get; }

        void ValidateParameters(IEnumerable<QueryParameter> parameters, bool subscription = false);
        Task<IEnumerable<IEntity>> Execute(IEnumerable<QueryParameter> parameters, IUnitOfWork unitOfWork);
    }
}
