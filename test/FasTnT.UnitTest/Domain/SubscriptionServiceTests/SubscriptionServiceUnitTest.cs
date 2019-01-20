using FakeItEasy;
using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Persistence;
using FasTnT.Domain.Services;
using FasTnT.Model.Queries.Implementations;
using FasTnT.UnitTest.Common;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    public abstract class BaseSubscriptionServiceUnitTest : BaseUnitTest
    {
        public IUnitOfWork UnitOfWork { get; set; } = A.Fake<IUnitOfWork>();
        public ISubscriptionBackgroundService SubscriptionBackgroundService { get; set; } = A.Fake<ISubscriptionBackgroundService>();
        public IEpcisQuery[] EpcisQueries { get; set; } = new IEpcisQuery[] { new SimpleEventQuery(), new SimpleMasterDataQuery() };
        public SubscriptionService SubscriptionService { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            SubscriptionService = new SubscriptionService(EpcisQueries, UnitOfWork, SubscriptionBackgroundService);
        }
    }
}
