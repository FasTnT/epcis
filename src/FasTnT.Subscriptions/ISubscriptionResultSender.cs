using FasTnT.Commands.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Subscriptions
{
    public interface ISubscriptionResultSender
    {
        Task<bool> Send(string destination, IEpcisResponse epcisResponse, CancellationToken cancellationToken);
    }
}
