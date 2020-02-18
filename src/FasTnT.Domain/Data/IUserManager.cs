using FasTnT.Model.Users;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Domain.Data
{
    public interface IUserManager
    {
        Task<User> GetByUsername(string username, CancellationToken cancellationToken); 
    }
}
