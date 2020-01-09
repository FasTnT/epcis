using System.Data;

namespace FasTnT.Domain.Data
{
    public interface ITransactionProvider
    {
        IDbTransaction BeginTransaction();
    }
}
