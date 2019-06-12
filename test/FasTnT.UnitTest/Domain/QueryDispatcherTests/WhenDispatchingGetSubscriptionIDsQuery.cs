using FasTnT.Model.Queries;
using FasTnT.Model.Responses;
using FasTnT.UnitTest.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasTnT.UnitTest.Domain.QueryDispatcherTests
{
    [TestClass]
    public class WhenDispatchingGetSubscriptionIDsQuery : QueryDispatcherFixture
    {
        public override void Arrange()
        {
            base.Arrange();

            Query = new GetSubscriptionIds();
        }

        [Assert]
        public void TheResponseShouldNotBeNull()
        {
            Assert.IsNotNull(Response);
        }

        [Assert]
        public void TheResponseShouldBeAGetSubscriptionIdsResult()
        {
            Assert.IsInstanceOfType(Response, typeof(GetSubscriptionIdsResult));
        }
    }
}
