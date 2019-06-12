using FasTnT.Model.Subscriptions;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingUnsubscribeRequest : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new UnsubscribeRequest();
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
