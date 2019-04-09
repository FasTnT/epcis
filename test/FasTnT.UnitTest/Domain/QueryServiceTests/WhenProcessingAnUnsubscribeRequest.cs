using FakeItEasy;
using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using FasTnT.UnitTest.Domain.QueryServiceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    [TestClass]
    public class WhenProcessingAnUnsubscribeRequest : BaseQueryServiceUnitTest
    {
        public UnsubscribeRequest Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new UnsubscribeRequest { SubscriptionId = "TestSubscription" };
        }

        public override void Act() => Task.WaitAll(QueryService.Unsubscribe(Request, default));

        [Assert]
        public void ItShouldCallTheSubscriptionManagerProperty() => A.CallTo(() => UnitOfWork.SubscriptionManager).MustHaveHappened();

        [Assert]
        public void ItShouldRemoveTheSubscriptionFromTheService() => A.CallTo(() => SubscriptionBackgroundService.Remove(A<Subscription>._)).MustHaveHappened();
    }
}
