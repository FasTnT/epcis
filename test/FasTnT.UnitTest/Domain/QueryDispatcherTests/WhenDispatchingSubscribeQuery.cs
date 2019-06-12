using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingSubscribeQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new Subscription
            {
                SubscriptionId = "unusedSubscriptionName",
                QueryName = "SimpleEventQuery",
                Destination = "http://unittest.destination.epcis.server/",
                Trigger = "TestTrigger"
            };
        }

        [Assert]
        public void TheResponseShouldBeNull()
        {
            Assert.IsNull(Response);
        }

        [Assert]
        public void ItShouldNotThrowAnException()
        {
            Assert.IsNull(Exception);
        }
    }
}
