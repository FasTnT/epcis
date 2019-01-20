using FasTnT.Domain.Services.Setup;

namespace FasTnT.Domain.Persistence
{
    public interface IUnitOfWork
    {
        IEventStore EventStore { get; }
        IEventRepository EventManager { get; }
        ISubscriptionManager SubscriptionManager { get; }
        IMasterDataManager MasterDataManager { get; }
        IDatabaseMigrator DatabaseManager { get; }
        IUserManager UserManager { get; }

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
