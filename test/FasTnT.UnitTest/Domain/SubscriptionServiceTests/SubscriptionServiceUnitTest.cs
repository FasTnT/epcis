using FakeItEasy;
using FasTnT.Domain.BackgroundTasks;
using FasTnT.Domain.Services;
using FasTnT.UnitTest.Common;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    public abstract class BaseSubscriptionServiceUnitTest : BaseUnitTest
    {
        public ISubscriptionBackgroundService SubscriptionBackgroundService { get; set; } = A.Fake<ISubscriptionBackgroundService>();
        public SubscriptionService SubscriptionService { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            SubscriptionService = new SubscriptionService(SubscriptionBackgroundService);
        }
    }
}
