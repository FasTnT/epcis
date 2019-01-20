using FasTnT.Domain.Persistence;
using FasTnT.Model.Users;
using System.Linq;
using System.Threading.Tasks;

namespace FasTnT.Persistence.Dapper
{
    public class PgSqlUserManager : IUserManager
    {
        private readonly DapperUnitOfWork _unitOfWork;

        public PgSqlUserManager(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<User> GetByUsername(string username)
            => (await _unitOfWork.Query<User>(SqlRequests.UserLoadByName, new { Username = username })).SingleOrDefault();
    }
}
