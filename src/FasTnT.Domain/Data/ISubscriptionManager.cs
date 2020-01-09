using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface ISubscriptionManager
    {
        Task<string[]> GetSubscriptionIds();
    }
}
