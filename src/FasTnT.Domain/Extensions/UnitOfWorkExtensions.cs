using FasTnT.Domain.Persistence;
using System;
using System.Threading.Tasks;

namespace FasTnT.Domain.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static async Task Execute(this IUnitOfWork unitOfWork, Func<IUnitOfWork, Task> action)
        {
            try
            {
                unitOfWork.BeginTransaction();

                await action(unitOfWork);
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
            }
        }
    }
}
