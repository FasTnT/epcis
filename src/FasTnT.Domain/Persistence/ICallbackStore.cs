using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface ICallbackStore
    {
        Task Store(Guid? requestId, string subscriptionId, QueryCallbackType callbackType);
    }
}
