using FakeItEasy;
using FasTnT.Domain.Services;
using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FasTnT.UnitTest.Domain.SubscriptionServiceTests
{
    [TestClass]
    public class WhenProcessingATriggerSubscriptionRequest : BaseSubscriptionServiceUnitTest
    {
        const string Trigger = "test-trigger";
        public TriggerSubscriptionRequest Request { get; set; }

        public override void Arrange()
        {
            base.Arrange();

            Request = new TriggerSubscriptionRequest { Trigger = Trigger };
        }

        public override void Act() => Task.WaitAll(SubscriptionService.TriggerSubscription(Request, default));

        [Assert]
        public void ItShouldHaveCalledTheSubscriptionServiceTriggerMethod() => A.CallTo(() => SubscriptionBackgroundService.Trigger(A<string>._)).MustHaveHappened();

        [Assert]
        public void ItShouldHaveCalledTheSubscriptionServiceTriggerMethodWithTheSpecifiedTrigger() => A.CallTo(() => SubscriptionBackgroundService.Trigger(Trigger)).MustHaveHappened();
    }
}
