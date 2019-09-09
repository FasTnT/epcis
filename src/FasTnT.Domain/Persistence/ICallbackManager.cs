using FasTnT.Model.Events.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ICallbackManager
    {
        Task Store(int? requestId, string subscriptionId, QueryCallbackType callbackType, CancellationToken cancellationToken);
    }
}
