using FasTnT.Model.Users;
using System.Threading.Tasks;

namespace FasTnT.Domain.Persistence
{
    public interface IUserManager
    {
        Task<User> GetByUsername(string username);
    }
}
