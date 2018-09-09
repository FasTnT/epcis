using System.Collections.Generic;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace FasTnT.Persistence.Dapper
{
    public interface IUnitOfWork
    {
        void Begin();
        void Commit();

        Task Execute(string command, object parameters);
        Task<IEnumerable<T>> Query<T>(string command, object parameters);
        Task<GridReader> FetchMany(string command, object parameters);
    }
}
